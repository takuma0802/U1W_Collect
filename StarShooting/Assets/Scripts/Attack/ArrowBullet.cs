﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ArrowBullet : BaseBullet, IDamageApplicable
{
    [SerializeField] float moveSpeed;
    Vector3 m_velocity; // 速度
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject _destroyExplosion;

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
        Destroy(gameObject);
    }

    public void ShootBullet(float angle)
    {
        // 弾が進行方向を向くようにする
        var angles = sprite.transform.localEulerAngles;
        angles.z = angle + 135;
        sprite.transform.localEulerAngles = angles;

        var direction = Utilities.GetDirection(angle);
        m_velocity = direction.normalized * moveSpeed;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.Translate(m_velocity * Time.deltaTime);

                // 画面外に出たら消滅
                if(!Utilities.CheckVisibllity(transform.position)) 
                {
                    Destroy(gameObject);
                }
                
            }).AddTo(this);
    }
}
