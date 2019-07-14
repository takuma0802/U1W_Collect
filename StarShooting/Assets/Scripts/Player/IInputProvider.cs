using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IInputProvider {
    IReadOnlyReactiveProperty<bool> AttackButtonDown { get; }
    IReadOnlyReactiveProperty<bool> AttackButtonUp { get; }
    IObservable<Vector2> MoveDirection { get; }
}