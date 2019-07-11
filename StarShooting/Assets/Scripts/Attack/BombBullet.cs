using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;
using DG.Tweening;

public class BombBullet : BaseBullet, IDamageApplicable
{
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject explosion; // Bombの爆発アニメーション
    [SerializeField] GameObject _destroyExplosion;
    [SerializeField] float explosionTime = 0.8f;

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
        var spriteR = sprite.GetComponent<SpriteRenderer>();
        spriteR.DOFade(0f, 0);
        spriteR.DOFade(1.0f, explosionTime / 2).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo);
        for (var i = 0; i < 3; i++)
        {
            // サウンド

            yield return new WaitForSeconds(explosionTime);
        }

        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
