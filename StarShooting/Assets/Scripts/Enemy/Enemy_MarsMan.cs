using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class Enemy_MarsMan : BaseEnemy
{
    [SerializeField] GameObject _myBulletPrefab;
    [SerializeField] GameObject _destroyExplosion;
    PlayerCore _core;
    Vector2 moveVelocity; // 自信の移動速度
    float _shotFrequency = 5f; // 弾の発射間隔
    float _angleRange = 40f; // 複数の弾を発射する時の角度

    public override void Init(SpownArea spwonType, float moveSpeed, PlayerCore core)
    {
        base.Init(spwonType, moveSpeed, core);
        _core = core;
        
        StartCoroutine(InitializeCoroutine());
    }

    IEnumerator InitializeCoroutine()
    {
        yield return Apear();
        _moveDirection = SwitchMoveDirection();
        moveVelocity = _moveDirection * _moveSpeed;
        this.UpdateAsObservable()
            .Where(x => _core != null)
            .Where(_ => _core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Subscribe(_ =>
            {
                transform.Translate(moveVelocity * Time.deltaTime);

                // 画面外に出たら消滅
                if(!Utilities.CheckVisibllity(transform.position)) 
                {
                    _isDead.Value = 2;
                    Destroy(gameObject);
                }

            }).AddTo(this.gameObject);

        // Playerに衝突したらダメージを与える
        this.OnTriggerEnter2DAsObservable()
            .Where(x => _core != null)
            .Where(_ => _core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Where(other => other.gameObject.GetComponent<PlayerCore>() != null)
            .Select(other => other.gameObject.GetComponent<IDamageApplicable>())
            .Where(damageApplicable => damageApplicable != null)
            .Subscribe(damageApplicable =>
            {
                damageApplicable.ApplyDamage(1);
            }).AddTo(this.gameObject);

        // 攻撃
        Observable.Interval(TimeSpan.FromSeconds(_shotFrequency))
            .Where(x => _core != null)
            .Where(_ => _core.CurrentGameState.CurrentGameState.Value == GameState.Main)
            .Subscribe(l =>
            {
                Shoot();
            }).AddTo(this.gameObject);
    }

    IEnumerator Apear()
    {
        transform.localScale = new Vector2(0,0);
        var moveSequence = transform.DOScale(Vector2.one,1f);
        yield return moveSequence.WaitForCompletion();
    }

    Vector2 SwitchMoveDirection()
    {
        switch (_spownArea)
        {
            case SpownArea.UPPER:
                return Vector2.down;

            case SpownArea.BOTTOM:
                return Vector2.up;

            case SpownArea.RIGHT:
                return Vector2.left;

            case SpownArea.LEFT:
                return Vector2.right;

            default:
                return Vector2.zero;
        }
    }

    public override void ApplyDamage(int power)
    {
        _isDead.Value = 1;
        Instantiate(_destroyExplosion,transform.position,Quaternion.identity);
        Destroy(this.gameObject);
    }

    public void Shoot()
    {
        for (int i = 0; i < 3; ++i)
        {
        // 弾の発射角度の計算
        var angleBase = Utilities.GetAngle(transform.position, _core.gameObject.transform.position);
        var angle = angleBase + _angleRange * (i - 1);

        var bullet = Instantiate(_myBulletPrefab, transform.localPosition, Quaternion.identity).GetComponent<ArrowBullet>();
        bullet.ShootBullet(angle);
        }
    }
}
