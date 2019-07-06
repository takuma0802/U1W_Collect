﻿using System;
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
    float moveSpeed = 0.3f;
    Vector3 m_velocity; // 速度

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
            }).AddTo(this.gameObject);

        // 敵に衝突した時、自身を消す
        this.OnTriggerEnter2DAsObservable()
            .Select(other => other.gameObject.GetComponent<BaseEnemy>())
            .Where(enemy => enemy != null)
            .Subscribe(enemy =>
            {
                DestroyMyself(0);
            }).AddTo(this.gameObject);
    }

    void DestroyMyself(float time)
    {
        Destroy(this.gameObject,time);
    }

    public void ShootBullet(Vector2 direction,Vector2 scale)
    {
        transform.localScale = scale;
        m_velocity = direction * moveSpeed;
        this.UpdateAsObservable()
            .Subscribe(_ => 
            {
                transform.Translate(m_velocity);
            }).AddTo(this);
        DestroyMyself(4);
    }
}