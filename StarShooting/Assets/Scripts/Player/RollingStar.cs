using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

// 発射前にPlayerの周りで回転している星
public class RollingStar : BaseBullet
{
    int _starSize = 1;
    PlayerCore core;

    IDisposable _isCharging;
    public bool IsCharging { get { return _isCharging != null; } }

    ReactiveProperty<bool> _onDestroyPlayer = new ReactiveProperty<bool>(); // 自滅判定用
    public IReadOnlyReactiveProperty<bool> OnDestroyPlayer { get { return _onDestroyPlayer; } }

    private Dictionary<int, float> _sizeDictionary = new Dictionary<int, float>() { { 1, 1f }, { 2, 1.2f }, { 3, 1.5f } };

    void Start()
    {
        // coreをGetすべし

        // 発射前に何かに衝突した時
        this.OnTriggerEnter2DAsObservable()
            .Where(other => other.gameObject.GetComponent<PlayerCore>() == null)
            .Select(other => other.gameObject.GetComponent<IDamageApplicable>())
            .Where(damageApplicable => damageApplicable != null)
            .Subscribe(damageApplicable =>
            {
                // 死亡
                DestroyPlayer();
            });
    }

    // Playerが死んだ時に呼ぶ
    void DestroyPlayer()
    {
        _onDestroyPlayer.Value = true;
    }

    public void ChargePower()
    {
        // 1秒ごとにサイズを大きくする
        _isCharging = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(l =>
            {
                PlusSize();
            }).AddTo(this);
    }

    void PlusSize()
    {
        _starSize++;
        Debug.Log("SizePlus!:" + _starSize);

        // 自滅チェック
        if (_starSize > 3)
        {
            DestroyPlayer();
            return;
        }

        ChangeSize();
    }

    // ButtonUp時に呼ばれ、自身を見えなくする
    public void ShootBullet()
    {
        StopChagingPower();
        gameObject.SetActive(false);
        _starSize = 1;
        ChangeSize();
    }

    void ChangeSize()
    {
        Vector2 nextSize = new Vector2(_sizeDictionary[_starSize],_sizeDictionary[_starSize]);
        gameObject.transform.DOScale(nextSize, 0.1f);
    }

    void StopChagingPower()
    {
        _isCharging.Dispose();
        _isCharging = null;
    }
}
