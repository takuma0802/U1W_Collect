using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IGameStateReadable {
    IReadOnlyReactiveProperty<GameState> CurrentGameState { get; }
}

public interface IWaveStateReadable {
    IReadOnlyReactiveProperty<WaveState> CurrentWaveState { get; }
}