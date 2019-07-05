using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

public class GameManager : MonoBehaviour, IGameStateReadable
{
    ReactiveProperty<GameState> _currentGameState = new ReactiveProperty<GameState>(GameState.Title);
    public IReadOnlyReactiveProperty<GameState> CurrentGameState { get { return _currentGameState; } }

    [SerializeField] Camera[] _cameras;
    [SerializeField] Canvas[] _canvases;
    [SerializeField] GameObject _playerPrefab;

    PlayerCore _player;

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
        switch (state)
        {
            case GameState.Title:
                TitleState();
                break;

            case GameState.Initialize:
                InitializeGameScene();
                break;

            case GameState.Main:
                PlayingGameState();
                break;

            case GameState.Result:
                ResultState();
                break;
        }
    }

    void TitleState()
    {
        _cameras[0].gameObject.SetActive(false);
        _canvases[0].gameObject.SetActive(false);
        _cameras[1].gameObject.SetActive(true);
        _canvases[1].gameObject.SetActive(true);
    }

    void InitializeGameScene()
    {
        _player = Instantiate(_playerPrefab).GetComponent<PlayerCore>();
        _player.Initialize();
        _currentGameState.Value = GameState.Main;
    }

    void PlayingGameState()
    {
        _player.IsDead
            .Where(x => x)
            .Subscribe(x =>
            {
                // 死亡処理
                _currentGameState.Value = GameState.Result;
            });
        

    }

    void ResultState()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(100);
        Invoke("OnClickTitleBackButton",5.0f);
    }

    public void OnClickStartButton()
    {
        _cameras[0].gameObject.SetActive(true);
        _canvases[0].gameObject.SetActive(true);
        _cameras[1].gameObject.SetActive(false);
        _canvases[1].gameObject.SetActive(false);
        _currentGameState.Value = GameState.Initialize;
    }

    public void OnClickTitleBackButton()
    {
        _currentGameState.Value = GameState.Title;
    }
}
