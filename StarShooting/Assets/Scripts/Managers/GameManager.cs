using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public enum GameState
{
    Title = 0,
    Initialize = 1,
    Main = 2,
    Result = 3
}

public enum WaveState
{
    Wave1 = 1,
    Wave2 = 2
}

public class GameManager : MonoBehaviour,IGameStateReadable
{
    ReactiveProperty<GameState> _currentGameState = new ReactiveProperty<GameState>(GameState.Title);
    public IReadOnlyReactiveProperty<GameState> CurrentGameState { get { return _currentGameState; } }

    void Start()
    {
        AudioManager.Instance.PlayBGM(BGM.BGM1.ToString(), 2f);


        CurrentGameState.Subscribe(state => 
        {
            ChangedGameState(state);
        });

    }

    void ChangedGameState(GameState state)
    {
        switch(state)
        {
            case GameState.Title:
                break;
            case GameState.Initialize:
                break;
            case GameState.Main:
                break;
            case GameState.Result:
                break;
                   
        }
    }
}
