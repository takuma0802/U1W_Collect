using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

// 発射後に生成される星
public class StarBullet : BaseBullet {
    int _damagePower = 1;
    float moveSpeed = 10f;
    Vector3 m_velocity; // 速度

    void Start () {
        // 敵か弾に衝突した時、そいつにダメージを与える
        this.OnTriggerEnter2DAsObservable ()
            .Where (other => other.gameObject.GetComponent<PlayerCore> () == null)
            .Select (other => other.gameObject.GetComponent<IDamageApplicable> ())
            .Where (damageApplicable => damageApplicable != null)
            .Subscribe (damageApplicable => {
                damageApplicable.ApplyDamage (_damagePower);
                AudioManager.Instance.PlaySE (SE.EnemyHit.ToString ());
            }).AddTo (this.gameObject);

        // 敵に衝突した時、自身を消す
        this.OnTriggerEnter2DAsObservable ()
            .Select (other => other.gameObject.GetComponent<BaseEnemy> ())
            .Where (enemy => enemy != null)
            .Subscribe (enemy => {
                DestroyMyself (0);
            }).AddTo (this.gameObject);
    }

    void DestroyMyself (float time) {
        Destroy (this.gameObject, time);
    }

    public void ShootBullet (Vector2 direction, Vector2 scale) {
        transform.localScale = scale;
        m_velocity = direction.normalized * moveSpeed;
        this.UpdateAsObservable ()
            .Subscribe (_ => {
                transform.Translate (m_velocity * Time.deltaTime);

                // 画面外に出たら消滅
                if (!Utilities.CheckVisibllity (transform.position)) {
                    Destroy (gameObject);
                }

            }).AddTo (this);
    }
}