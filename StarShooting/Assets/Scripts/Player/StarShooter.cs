using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class StarShooter : MonoBehaviour
{
    [SerializeField] GameObject StarBox;
    [SerializeField] GameObject StarBulletPrefab;
    [SerializeField] RollingStar[] HoldingStars; // Playerの周りを回転している星
    [SerializeField] GameObject[] HoldingStarArrows; // 星の発射矢印
    [SerializeField] float rotateSpeed;

    ReactiveProperty<int> starNum = new ReactiveProperty<int>(0);  // Starの保有数
    ReactiveProperty<int> nextStar = new ReactiveProperty<int>(0); // 次発射するStar番号

    PlayerCore core;
    CompositeDisposable _compositeDisposable = new CompositeDisposable();　// 保有している星のダメージ監視リスト

    void Awake()
    {
        core = GetComponent<PlayerCore>();
        core.InitializeSubject
            .Subscribe(_ =>
            {
                Initialize();
            });
    }

    void Initialize()
    {
        _compositeDisposable.Clear();
        starNum.Value = 0;
        nextStar.Value = 0;
        //rotateSpeed = 170f;

        for(var i = 0; i < HoldingStars.Length; i++)
        {
            HoldingStars[i].core = core;
            var stream = HoldingStars[i].OnDamaged
                        .Subscribe(_ => PlayerDamaged());

            _compositeDisposable.Add(stream);
            HoldingStars[i].gameObject.SetActive(false);
            HoldingStarArrows[i].SetActive(false);
        }
        HoldingStarArrows[0].SetActive(true);

        // 星の巨大化
        core.InputProvider.AttackButtonDown
            .Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Where(x => x)
            .Where(x => starNum.Value > 0)
            .Subscribe(x =>
            {
                ChargingPower();
            }).AddTo(this.gameObject);

        // 星の発射
        core.InputProvider.AttackButtonUp
            .Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Where(x => x)
            .Where(_ => HoldingStars[nextStar.Value].IsCharging)
            .Subscribe(x =>
            {
                ShootStar();
            }).AddTo(this.gameObject);

        // 星を取得
        core.GetStarSubject
            .Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Subscribe(_ =>
            {
                AddStar();
            }).AddTo(this.gameObject);
        
        // 星の回転
        this.UpdateAsObservable()
            .Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Subscribe(_ =>
            {
                StarBox.transform.Rotate(-Vector3.forward, Time.deltaTime * rotateSpeed);
            }).AddTo(this.gameObject);
    }

    void PlayerDamaged()
    {
        core.ApplyDamage(1);
    }

    public void AddStar()
    {
        if (starNum.Value >= 7) return;

        var addNum = nextStar.Value + starNum.Value;
        if (addNum >= 7) addNum -= 7;
        HoldingStars[addNum].gameObject.SetActive(true);
        starNum.Value++;
    }

    void ChargingPower()
    {
        HoldingStars[nextStar.Value].ChargePower();
    }

    void ShootStar()
    {
        var nextStarTransform = HoldingStars[nextStar.Value].gameObject.transform;
        var angle = Utilities.GetAngle(transform.position, nextStarTransform.position);
        var direction = Utilities.GetDirection(angle);

        var bullet = Instantiate(StarBulletPrefab, nextStarTransform.position, Quaternion.identity).GetComponent<StarBullet>();
        bullet.ShootBullet(direction, nextStarTransform.localScale);
        HoldingStars[nextStar.Value].ShootBullet();
        AudioManager.Instance.PlaySE(SE.Shot.ToString());

        starNum.Value--;
        HoldingStarArrows[nextStar.Value].SetActive(false); // 発射後、矢印を表示しない

        nextStar.Value++;
        if (nextStar.Value >= 7) nextStar.Value = 0;
        HoldingStarArrows[nextStar.Value].SetActive(true); // 次の星の矢印表示
    }
}
