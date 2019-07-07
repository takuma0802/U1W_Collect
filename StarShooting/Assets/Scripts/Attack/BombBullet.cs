using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using DG.Tweening;

public class BombBullet : BaseBullet, IDamageApplicable
{
    Vector3 m_velocity; // 速度
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject explosion;

    void Start()
    {
        // Playerに衝突したらダメージを与える
        this.OnTriggerEnter2DAsObservable()
            .Where(other => other.gameObject.GetComponent<PlayerCore>() != null)
            .Select(other => other.gameObject.GetComponent<IDamageApplicable>())
            .Where(damageApplicable => damageApplicable != null)
            .Subscribe(damageApplicable =>
            {
                damageApplicable.ApplyDamage(1);
                ApplyDamage(1);
            }).AddTo(this.gameObject);
    }

    [SerializeField] GameObject _destroyExplosion;
    public void ApplyDamage(int power)
    {
        var explosion = Instantiate(_destroyExplosion, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector2(1.5f, 1.5f);
        Destroy(this.gameObject);
    }

    public void ShootBullet()
    {
        StartCoroutine(ExplosionCoroutine());
    }

    IEnumerator ExplosionCoroutine()
    {
        var time = 0.8f;
        var spriteR = sprite.GetComponent<SpriteRenderer>();
        spriteR.DOFade(0f, 0);
        spriteR.DOFade(1.0f, time / 2).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo);
        for (var i = 0; i < 3; i++)
        {
            // サウンド

            yield return new WaitForSeconds(time);
        }

        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
