using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

// 発射前にPlayerの周りで回転している星
public class RollingStar : BaseBullet, IDamageApplicable {
    [HideInInspector] public PlayerCore core;
    [SerializeField] GameObject _destroyExplosion;

    IDisposable _isCharging;
    public bool IsCharging { get { return _isCharging != null; } }

    Subject<Unit> _onDamaged = new Subject<Unit> (); // 自滅判定用
    public IObservable<Unit> OnDamaged { get { return _onDamaged; } }

    Dictionary<int, float> _sizeDictionary = new Dictionary<int, float> () { { 0, 0.56f }, { 1, 0.8f }, { 2, 1f }, { 3, 1.2f } };
    int _starSize = 0; // 現在の星の大きさレベル (Max3)

    void Start () {
        // 発射前に何かに衝突した場合の判定
        this.OnTriggerEnter2DAsObservable ()
            .Where (_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main) // InGameかどうか
            .Where (other => other.gameObject.GetComponent<PlayerCore> () == null) // Playerだったら無視
            .Select (other => other.gameObject.GetComponent<IDamageApplicable> ())
            .Where (damageApplicable => damageApplicable != null)
            .Subscribe (damageApplicable => {
                ApplyDamage (1);
                damageApplicable.ApplyDamage (1);
            }).AddTo (this.gameObject);
    }

    public void ApplyDamage (int damage) {
        var explosion = Instantiate (_destroyExplosion, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector2 (1.5f, 1.5f);
        _onDamaged.OnNext (Unit.Default);
    }

    public void ChargePower () {
        // 1秒ごとにサイズを大きくする
        _isCharging = Observable.Interval (TimeSpan.FromSeconds (0.8f)).Subscribe (l => {
            PlusSize ();
        }).AddTo (this);
    }

    void PlusSize () {
        _starSize++;
        // 自滅チェック
        if (_starSize > 3) {
            ApplyDamage (1);
            _starSize = 0;
            return;
        }

        ChangeSize ();
    }

    // ButtonUp時(星発射時)に呼ばれ、自身を見えなくする
    public void ShootBullet () {
        StopChagingPower ();
        gameObject.SetActive (false);
        _starSize = 0;
        ChangeSize ();
    }

    void ChangeSize () {
        Vector2 nextSize = new Vector2 (_sizeDictionary[_starSize], _sizeDictionary[_starSize]);
        gameObject.transform.DOScale (nextSize, 0.1f);
    }

    void StopChagingPower () {
        _isCharging.Dispose ();
        _isCharging = null;
        _starSize = 0;
    }
}