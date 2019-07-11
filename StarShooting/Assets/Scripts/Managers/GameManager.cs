using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public enum GameState
{
    Title = 0,
    Initialize = 1,
    Main = 2,
    Result = 3,
    Ranking = 4
}

public enum WaveState
{
    Wave0 = 0,
    Wave1 = 1,
    Wave2 = 2,
    Wave3 = 3,
    Wave4 = 4,
    Wave5 = 5
}

public class GameManager : MonoBehaviour, IGameStateReadable, IWaveStateReadable
{
    ReactiveProperty<GameState> _currentGameState = new ReactiveProperty<GameState>(GameState.Title);
    public IReadOnlyReactiveProperty<GameState> CurrentGameState { get { return _currentGameState; } }

    // Wave管理
    [SerializeField] int[] WavePointOfKilledEnemy = new int[] { };
    [SerializeField] int[] WavePointOfSpendTime = new int[] { };
    ReactiveProperty<WaveState> _currentWave = new ReactiveProperty<WaveState>(WaveState.Wave0);
    public IReadOnlyReactiveProperty<WaveState> CurrentWaveState { get { return _currentWave; } }


    [SerializeField] Camera[] _cameras;
    [SerializeField] Canvas[] _canvases;
    [SerializeField] GameObject _playerPrefab;

    protected PlayerCore _playerCore;
    EnemyController _enemyController;
    TimeManager _timeManager;
    ScoreManager _scoreManager;
    StarGenerator _starGenerator;
    GameView _gameView;
    int _currentScore;


    void Start()
    {
        AudioManager.Instance.PlayBGM(BGM.BGM1.ToString(), 2f);
        _enemyController = GetComponent<EnemyController>();
        _timeManager = GetComponent<TimeManager>();
        _scoreManager = GetComponent<ScoreManager>();
        _starGenerator = GetComponent<StarGenerator>();
        _gameView = GetComponent<GameView>();

        CurrentGameState.Subscribe(state =>
        {
            ChangedGameState(state);
        }).AddTo(this.gameObject);
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

            case GameState.Ranking:
                RankingState();
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

    public void InitializeGameScene()
    {
        _gameView.ResetView();
        _playerCore = Instantiate(_playerPrefab,Vector2.zero,Quaternion.identity).GetComponent<PlayerCore>();
        _playerCore.Initialize(this);
        _enemyController.Initialize(this, _playerCore);
        _starGenerator.Initialize(this, _playerCore);
        _timeManager.Reset(this);
        
        _currentWave.Value = WaveState.Wave0;
        _currentGameState.Value = GameState.Main;
    }

    void PlayingGameState()
    {
        _timeManager.StartGameTimer();
        UpdateGameView();
        UpdateWaveState();

        _playerCore.IsDead
            .Where(x => x)
            .Subscribe(x =>
            {
                _currentGameState.Value = GameState.Result;
            });
    }

    void UpdateGameView()
    {
        _playerCore.PlayerLife
            .SkipLatestValueOnSubscribe()
            .Where(x => x >= 0)
            .TakeWhile(_ => CurrentGameState.Value != GameState.Result)
            .Subscribe(x => _gameView.CutLifeView(x));
        
        _enemyController.KillNumber
            .TakeWhile(_ => CurrentGameState.Value != GameState.Result)
                .Subscribe(x => _gameView.UpdateEnemyView(x));

        _timeManager.PassedTime
            .TakeWhile(_ => CurrentGameState.Value != GameState.Result)
            .Subscribe(x => _gameView.UpdateTimeView(x));

        _starGenerator.GetStarNumber
            .TakeWhile(_ => CurrentGameState.Value != GameState.Result)
            .Subscribe(x => _gameView.UpdateStarView(x));

        CurrentWaveState
            .Where(_ => CurrentGameState.Value == GameState.Main)
            .Subscribe(x => _gameView.UpdateWaveView(x));
    }

    void UpdateWaveState()
    {
        // Wave管理
        _enemyController.KillNumber
            .Where(x => CurrentGameState.Value == GameState.Main)
            .Where(x => x > WavePointOfKilledEnemy[(int)_currentWave.Value])
            .TakeWhile(_ => CurrentGameState.Value != GameState.Result)
            .Subscribe(_ => PlusWaveState());
            
        _timeManager.PassedTime
            .Where(x => CurrentGameState.Value == GameState.Main)
            .Where(x => x > WavePointOfSpendTime[(int)_currentWave.Value])
            .TakeWhile(_ => CurrentGameState.Value != GameState.Result)
            .Subscribe(_ => PlusWaveState());
    }

    void PlusWaveState()
    {
        var next = (int)CurrentWaveState.Value + 1;
        _currentWave.Value = Utilities.ConvertToEnum<WaveState>(next);
    }

    void ResultState()
    {
        var time = _timeManager.PassedTime.Value;
        var enemy = _enemyController.KillNumber.Value;
        var allEnemy = _enemyController.AllGeneratedEnemies.Value;
        var star = _starGenerator.GetStarNumber.Value;
        int[] scores = _scoreManager.CulcScore(time,enemy,allEnemy,star);
        _currentScore = scores[3];
        _gameView.ShowResult(scores,time,enemy,allEnemy,star);

        _enemyController.CreaAllEnemyStream();
        _starGenerator.CreaAllEnemyStream();
        _playerCore.CreaPlayer();
    }

    void RankingState()
    {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(_currentScore);
    }

    public void OnClickStartButton()
    {
        AudioManager.Instance.PlaySE(SE.Click.ToString());
        _cameras[0].gameObject.SetActive(true);
        _canvases[0].gameObject.SetActive(true);
        _cameras[1].gameObject.SetActive(false);
        _canvases[1].gameObject.SetActive(false);
        _currentGameState.Value = GameState.Initialize;
    }

    public void OnClickTitleBackButton()
    {
        AudioManager.Instance.PlaySE(SE.Click.ToString());
        _currentGameState.Value = GameState.Title;
    }

    public void OnClickRankingButton()
    {
        AudioManager.Instance.PlaySE(SE.Click.ToString());
        _currentGameState.Value = GameState.Ranking;
    }
}
