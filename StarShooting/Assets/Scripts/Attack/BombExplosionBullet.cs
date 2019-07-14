using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BombExplosionBullet : BaseBullet, IDamageApplicable {
    void Start () {
        // Playerに衝突したらダメージを与える
        this.OnTriggerEnter2DAsObservable ()
            .Where (other => other.gameObject.GetComponent<PlayerCore> () != null)
            .Select (other => other.gameObject.GetComponent<IDamageApplicable> ())
            .Where (damageApplicable => damageApplicable != null)
            .Subscribe (damageApplicable => {
                damageApplicable.ApplyDamage (1);
                ApplyDamage (1);
            }).AddTo (this.gameObject);

        Destroy (gameObject, 0.2f);
    }

    public void ApplyDamage (int power) {

    }
}