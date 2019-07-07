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
    [SerializeField] float _shotFrequency = 4f; // 弾の発射間隔
    [SerializeField] float _oneShotFrequency = 0.4f; // 1回の発射間隔

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
            case SpownArea.UPPER:
                return new Vector2(-1,-1);

            case SpownArea.BOTTOM:
                return new Vector2(1,1);

            case SpownArea.RIGHT:
                return new Vector2(-1,-0.5f);

            case SpownArea.LEFT:
                return new Vector2(1,0.5f);
            
            case SpownArea.UPPERLEFT:
                return new Vector2(1,0);

            case SpownArea.BOTTOMLEFT:
                return new Vector2(1,0);

            case SpownArea.UPPERRIGHT:
                return new Vector2(-1,0);

            case SpownArea.BOTTOMRIGHT:
                return new Vector2(0,1);

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
        AudioManager.Instance.PlaySE(SE.Bullet.ToString());

        for (int i = 0; i < 3; ++i)
        {
            var bullet = Instantiate(_myBulletPrefab, transform.localPosition, Quaternion.identity).GetComponent<BulletBullet>();
            bullet.ShootBullet(angle);
            yield return new WaitForSeconds(_oneShotFrequency);
        }
    }
}
