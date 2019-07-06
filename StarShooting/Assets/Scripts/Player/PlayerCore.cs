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

    ReactiveProperty<bool> _isDead = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> IsDead { get { return _isDead; } }


    void Awake()
    {
        _inputProvider = GetComponent<IInputProvider>();
    }
    void Start()
    {
        //Initialize();
    }

    public void Initialize(IGameStateReadable gameState)
    {
        _currentGameState = gameState;

        this.OnTriggerEnter2DAsObservable()
            .Select(other => other.gameObject.GetComponent<StarItem>())
            .Where(star => star != null)
            .Subscribe(star =>
            {
                _getStarSubject.OnNext(Unit.Default);
                star.DestroyStar();
            }).AddTo(this.gameObject);

        // その他のクラスの初期化
        _initializeSubject.OnNext(Unit.Default);
        _initializeSubject.OnCompleted();
    }

    public void ApplyDamage(int power)
    {
        // 死亡通知
        Debug.Log("GameOver!");
        _isDead.Value = true;
    }
}
