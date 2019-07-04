using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class KeyInputProvider : MonoBehaviour,IInputProvider
{
    private ReactiveProperty<bool> _attackButtonDown = new ReactiveProperty<bool>();
    private ReactiveProperty<bool> _attackButtonUp = new ReactiveProperty<bool>();
    private ReactiveProperty<Vector2> _moveDirection = new ReactiveProperty<Vector2>();
    public IReadOnlyReactiveProperty<bool> AttackButtonDown { get { return _attackButtonDown;}}
    public IReadOnlyReactiveProperty<bool> AttackButtonUp { get { return _attackButtonUp;}}
    public IReadOnlyReactiveProperty<Vector2> MoveDirection { get { return _moveDirection;}}

    // Update is called once per frame
    public void Start()
    {
        this.UpdateAsObservable()
            .Select(_ => Input.GetKeyDown(KeyCode.Space))
            .DistinctUntilChanged()
            .Subscribe(x => _attackButtonDown.Value = x);
            
        this.UpdateAsObservable()
            .Select(_ => Input.GetKeyUp(KeyCode.Space))
            .DistinctUntilChanged()
            .Subscribe(x => _attackButtonUp.Value = x);

        this.UpdateAsObservable()
            .Select(_ => new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")))
            .Subscribe(x => _moveDirection.Value = x);
    }
}
