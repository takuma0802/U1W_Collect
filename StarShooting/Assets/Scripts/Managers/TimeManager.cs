using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    private IntReactiveProperty _readyCountDownTime = new IntReactiveProperty (3);
    private ReactiveProperty<int> _passedTime = new ReactiveProperty<int> (0);

    public IReadOnlyReactiveProperty<int> ReadyTime {
        get { return _readyCountDownTime; }
    }

    public IReadOnlyReactiveProperty<int> PassedTime {
        get { return _passedTime; }
    }

    IGameStateReadable _gameState;

    public void Reset (IGameStateReadable state) {
        _gameState = state;
        _passedTime.Value = 0;
    }

    public void StartGameReadyCountDown () {
        StartCoroutine (ReadyCountCoroutine ());
    }

    IEnumerator ReadyCountCoroutine () {
        yield return new WaitForSeconds (0.5f);

        _readyCountDownTime.SetValueAndForceNotify (_readyCountDownTime.Value);

        yield return new WaitForSeconds (1);
        while (_readyCountDownTime.Value > 0) {
            _readyCountDownTime.Value -= 1;
            yield return new WaitForSeconds (1);
        }
    }

    public void StartGameTimer () {
        StartCoroutine (BattleCountDownCoroutine ());
    }

    IEnumerator BattleCountDownCoroutine () {
        while (_gameState.CurrentGameState.Value == GameState.Main) {
            yield return new WaitForSeconds (1f);
            _passedTime.Value += 1;
        }
    }
}