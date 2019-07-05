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
    float rotateSpeed = 200f;

    public RollingStar[] HoldingStars = new RollingStar[7];

    ReactiveProperty<int> starNum = new ReactiveProperty<int>(0);  // Starの保有数
    ReactiveProperty<int> nextStar = new ReactiveProperty<int>(0); // 次発射するStar

    PlayerCore core;
    CompositeDisposable _compositeDisposable = new CompositeDisposable();　// 保有している星の監視用リスト

    void Start()
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
        rotateSpeed = 200f;

        foreach (var star in HoldingStars)
        {
            var stream = star.OnDestroyPlayer
                        .Where(x => x)
                        .Subscribe(_ => PlayerDamaged());

            _compositeDisposable.Add(stream);
            star.gameObject.SetActive(false);
        }

        core.InputProvider.AttackButtonDown
            //.Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Where(x => x)
            .Where(x => starNum.Value > 0)
            .Subscribe(x =>
            {
                Debug.Log("ButtonDown！");
                ChargingPower();
            });

        core.InputProvider.AttackButtonUp
            //.Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Where(x => x)
            .Where(_ => HoldingStars[nextStar.Value].IsCharging)
            .Subscribe(x =>
            {
                Debug.Log("ButtonUp！");
                ShootStar();
            });

        core.GetStarSubject
            //.Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Subscribe(_ =>
            {
                AddStar();
            });

        this.UpdateAsObservable()
            //.Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Subscribe(_ =>
            {
                StarBox.transform.Rotate(-Vector3.forward, Time.deltaTime * rotateSpeed);
            });
    }

    // Player死亡
    void PlayerDamaged()
    {
        core.ApplyDamage(1);
        _compositeDisposable.Clear();
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
        var result = CulcShootAngle();
        var bullet = Instantiate(StarBulletPrefab,HoldingStars[nextStar.Value].gameObject.transform.position,Quaternion.identity).GetComponent<StarBullet>();
        bullet.ShootBullet(result);
        HoldingStars[nextStar.Value].ShootBullet();
        nextStar.Value++;
        if (nextStar.Value >= 7) nextStar.Value -= 7;
        starNum.Value--;
    }

    float[] CulcShootAngle()
    {
        Vector2 playerPos = transform.position;
        Vector2 starPos = HoldingStars[nextStar.Value].gameObject.transform.position;
        
        float xdiff = starPos.x - playerPos.x;
        float ydiff = starPos.y - playerPos.y;
        float ratio = Mathf.Abs(ydiff / xdiff);
        var result = new float[3]{xdiff,ydiff,ratio};
        return result;
    }
}
