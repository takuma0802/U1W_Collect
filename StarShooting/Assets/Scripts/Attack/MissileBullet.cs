using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class MissileBullet : BaseBullet, IDamageApplicable
{
    [SerializeField] float moveSpeed;
    [SerializeField] float _angleFrequency;
    [SerializeField] int _changeAngleNum;
    Vector3 m_velocity; // 速度
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject _destroyExplosion;
    GameObject _targetObject;

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

    public void ShootBullet(GameObject target)
    {
        _targetObject = target;
        StartCoroutine(MissileShotCoroutine());

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.Translate(m_velocity * Time.deltaTime);

                // 画面外に出たら消滅
                if (!Utilities.CheckVisibllity(transform.position))
                {
                    Destroy(gameObject);
                }

            }).AddTo(this);
    }

    IEnumerator MissileShotCoroutine()
    {
        for (var i = 0; i < _changeAngleNum; i++)
        {
            if(_targetObject == null) yield break;
            // 弾の発射角度の計算
            var angle = Utilities.GetAngle(transform.position, _targetObject.transform.position);

            // 弾が進行方向を向くようにする
            var angles = sprite.transform.localEulerAngles;
            angles.z = angle - 45;
            sprite.transform.localEulerAngles = angles;

            var direction = Utilities.GetDirection(angle);
            m_velocity = direction.normalized * moveSpeed;
            yield return new WaitForSeconds(_angleFrequency);
        }
    }
}