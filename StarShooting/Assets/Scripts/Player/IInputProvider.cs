using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IInputProvider
{
    IReadOnlyReactiveProperty<bool> AttackButtonDown { get; }
    IReadOnlyReactiveProperty<bool> AttackButtonUp { get; }
    IReadOnlyReactiveProperty<Vector2> MoveDirection { get; }
}