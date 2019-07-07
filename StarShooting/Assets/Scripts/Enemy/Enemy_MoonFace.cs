using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class Enemy_MoonFace : BaseEnemy
{
    [SerializeField] GameObject _myBulletPrefab;
    [SerializeField] GameObject _destroyExplosion;
    PlayerCore _core;
    Vector2 moveVelocity; // 自信の移動速度
    float _shotFrequency = 4f; // 弾の発射間隔
    float _oneShotFrequency = 0.4f; // 弾の発射間隔

    public override void Init(SpownArea spwonType, float moveSpeed, PlayerCore core)
    {
        base.Init(spwonType, moveSpeed, core);
        _core = core;

        StartCoroutine(InitializeCoroutine());
    }

    IEnumerator InitializeCoroutine()
    {
        yield return Apear();
        // 移動
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
                StartCoroutine(Shoot());
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
            case SpownArea.UPPERLEFT:
                return Vector2.down;

            case SpownArea.BOTTOMRIGHT:
                return Vector2.up;

            case SpownArea.UPPERRIGHT:
                return Vector2.left;

            case SpownArea.BOTTOMLEFT:
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

    public IEnumerator Shoot()
    {
        // 弾の発射角度の計算
        var angle = Utilities.GetAngle(transform.position, _core.gameObject.transform.position);

        for (int i = 0; i < 3; ++i)
        {
            var bullet = Instantiate(_myBulletPrefab, transform.localPosition, Quaternion.identity).GetComponent<BulletBullet>();
            bullet.ShootBullet(angle);
            yield return new WaitForSeconds(_oneShotFrequency);
        }
    }
}
