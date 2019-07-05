using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public interface IGameStateReadable
{
    IReadOnlyReactiveProperty<GameState> CurrentGameState { get; }
}