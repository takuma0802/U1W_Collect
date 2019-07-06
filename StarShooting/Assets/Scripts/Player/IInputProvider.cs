using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public interface IInputProvider
{
    IReadOnlyReactiveProperty<bool> AttackButtonDown { get; }
    IReadOnlyReactiveProperty<bool> AttackButtonUp { get; }
    IObservable<Vector2> MoveDirection { get; }
}