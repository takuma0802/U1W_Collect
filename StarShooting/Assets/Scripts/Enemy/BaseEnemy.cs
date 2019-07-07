using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public abstract class BaseEnemy : MonoBehaviour, IDamageApplicable
{
    protected Vector2 currentPos;
    protected Vector2 targetPos;
    protected float _moveSpeed;
    protected Vector2 _moveDirection;
    protected SpownArea _spownArea;
    protected ReactiveProperty<int> _isDead = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> IsDead { get { return _isDead; } }

    public virtual void Init(SpownArea spwonType, float moveSpeed, PlayerCore core)
    {
        this._spownArea = spwonType;
        this._moveSpeed = moveSpeed;
    }

    public abstract void ApplyDamage(int power);
}