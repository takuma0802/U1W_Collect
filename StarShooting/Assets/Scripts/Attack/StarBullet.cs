using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

// 発射後に生成される星
public class StarBullet : BaseBullet
{
    public bool WasFired = false; // 発射されているかどうか
    int _damagePower = 1;
    float moveSpeed = 8f;

    ReactiveProperty<bool> _onDestroyPlayer = new ReactiveProperty<bool>(); // 自滅判定用
    public IReadOnlyReactiveProperty<bool> OnDestroyPlayer { get { return _onDestroyPlayer; } }


    void Start()
    {
        // 敵か球に衝突した時、そいつにダメージを与える
        this.OnTriggerEnter2DAsObservable()
            .Where(other => other.gameObject.GetComponent<PlayerCore>() == null)
            .Select(other => other.gameObject.GetComponent<IDamageApplicable>())
            .Where(damageApplicable => damageApplicable != null)
            .Subscribe(damageApplicable =>
            {
                damageApplicable.ApplyDamage(_damagePower);
            });

        // 敵に衝突した時、自身を消す
        this.OnTriggerEnter2DAsObservable()
            .Select(other => other.gameObject.GetComponent<BaseEnemy>())
            .Where(enemy => enemy != null)
            .Subscribe(enemy =>
            {
                DestroyMyself();
            });
    }

    void DestroyMyself()
    {
        Destroy(this.gameObject);
    }

    public void ShootBullet(float[] result)
    {
        var x = Mathf.Sign(result[0]);
        var y = Mathf.Sign(result[1]);

        this.UpdateAsObservable()
            .Subscribe(_ => 
            {
                var xDir = Time.deltaTime * moveSpeed * x;
                var yDir = Time.deltaTime * moveSpeed * y * result[2];
                transform.Translate(new Vector2(xDir, yDir));
            }).AddTo(this);
    }
}
