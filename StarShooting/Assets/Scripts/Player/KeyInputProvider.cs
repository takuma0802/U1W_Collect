using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class KeyInputProvider : MonoBehaviour, IInputProvider {
    private ReactiveProperty<bool> _attackButtonDown = new ReactiveProperty<bool> ();
    private ReactiveProperty<bool> _attackButtonUp = new ReactiveProperty<bool> ();
    private Subject<Vector2> _moveDirection = new Subject<Vector2> ();
    public IReadOnlyReactiveProperty<bool> AttackButtonDown { get { return _attackButtonDown; } }
    public IReadOnlyReactiveProperty<bool> AttackButtonUp { get { return _attackButtonUp; } }
    public IObservable<Vector2> MoveDirection { get { return _moveDirection; } }

    // Update is called once per frame
    public void Awake () {
        this.UpdateAsObservable ()
            .Select (_ => Input.GetKeyDown (KeyCode.Space))
            .DistinctUntilChanged ()
            .Subscribe (x => _attackButtonDown.Value = x)
            .AddTo (this.gameObject);

        this.UpdateAsObservable ()
            .Select (_ => Input.GetKeyUp (KeyCode.Space))
            .DistinctUntilChanged ()
            .Subscribe (x => _attackButtonUp.Value = x)
            .AddTo (this.gameObject);

        this.UpdateAsObservable ()
            .Select (_ => new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical")))
            .Subscribe (x => _moveDirection.OnNext (x))
            .AddTo (this.gameObject);

    }
}