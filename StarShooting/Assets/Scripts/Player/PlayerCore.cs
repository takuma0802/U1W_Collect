using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerCore : MonoBehaviour, IDamageApplicable
{
    Subject<Unit> _initializeSubject = new Subject<Unit>();
    public IObservable<Unit> InitializeSubject { get { return _initializeSubject; } }

    IInputProvider _inputProvider;
    public IInputProvider InputProvider { get { return _inputProvider; } }

    IGameStateReadable _currentGameState;
    public IGameStateReadable CurrentGameState { get { return _currentGameState; } }

    Subject<Unit> _getStarSubject = new Subject<Unit>();
    public IObservable<Unit> GetStarSubject { get { return _getStarSubject; } }

    void Start()
    {
        _inputProvider = GetComponent<IInputProvider>();
        Initialize();
    }

    void Initialize()
    {
        this.OnTriggerEnter2DAsObservable()
            .Select(other => other.gameObject.GetComponent<StarItem>())
            .Where(star => star != null)
            .Subscribe(star =>
            {
                _getStarSubject.OnNext(Unit.Default);
                star.DestroyStar();
            });

        // その他のクラスの初期化
        _initializeSubject.OnNext(Unit.Default);
        _initializeSubject.OnCompleted();
    }

    public void ApplyDamage(int power)
    {
        // 死亡通知
        Debug.Log("GameOver!");
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking (100);
    }    
}
