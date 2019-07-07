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
    float rotateSpeed = 170f;

    [SerializeField] RollingStar[] HoldingStars;
    [SerializeField] GameObject[] HoldingStarArrows;

    ReactiveProperty<int> starNum = new ReactiveProperty<int>(0);  // Starの保有数
    ReactiveProperty<int> nextStar = new ReactiveProperty<int>(0); // 次発射するStar

    PlayerCore core;
    CompositeDisposable _compositeDisposable = new CompositeDisposable();　// 保有している星の監視用リスト

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
        rotateSpeed = 170f;
        HoldingStarArrows[0].SetActive(true);

        foreach (var star in HoldingStars)
        {
            star.core = core;
            var stream = star.OnDamaged
                        .Subscribe(_ => PlayerDamaged());

            _compositeDisposable.Add(stream);
            star.gameObject.SetActive(false);
        }

        core.InputProvider.AttackButtonDown
            .Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Where(x => x)
            .Where(x => starNum.Value > 0)
            .Subscribe(x =>
            {
                ChargingPower();
            }).AddTo(this.gameObject);

        core.InputProvider.AttackButtonUp
            .Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Where(x => x)
            .Where(_ => HoldingStars[nextStar.Value].IsCharging)
            .Subscribe(x =>
            {
                ShootStar();
            }).AddTo(this.gameObject);

        core.GetStarSubject
            .Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Subscribe(_ =>
            {
                AddStar();
            }).AddTo(this.gameObject);

        this.UpdateAsObservable()
            .Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Subscribe(_ =>
            {
                StarBox.transform.Rotate(-Vector3.forward, Time.deltaTime * rotateSpeed);
            }).AddTo(this.gameObject);

        nextStar.Subscribe(x =>
        {
            if (starNum.Value > 0)
            {
                HoldingStarArrows[x].SetActive(true);
            }

            if (x == 0) x = 7;
            HoldingStarArrows[x - 1].SetActive(false);
        });


    }

    void PlayerDamaged()
    {
        core.ApplyDamage(1);
    }

    public void AddStar()
    {
        if (starNum.Value >= 7) return;

        var addNum = starNum.Value + nextStar.Value;
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
        var nextStarObject = HoldingStars[nextStar.Value].gameObject.transform;
        var angle = Utilities.GetAngle(transform.position, nextStarObject.position);
        var direction = Utilities.GetDirection(angle);

        var bullet = Instantiate(StarBulletPrefab, nextStarObject.position, Quaternion.identity).GetComponent<StarBullet>();
        bullet.ShootBullet(direction, nextStarObject.localScale);
        HoldingStars[nextStar.Value].ShootBullet();
        AudioManager.Instance.PlaySE(SE.Shot.ToString());

        starNum.Value--;

        if (nextStar.Value + 1 >= 7)
        {
            nextStar.Value = 0;
        }
        else
        {
            nextStar.Value++;
        }
    }
}
