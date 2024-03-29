using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PlayerCore : MonoBehaviour, IDamageApplicable {
    [SerializeField] GameObject _destroyExplosion;

    Subject<Unit> _initializeSubject = new Subject<Unit> ();
    public IObservable<Unit> InitializeSubject { get { return _initializeSubject; } }

    IInputProvider _inputProvider;
    public IInputProvider InputProvider { get { return _inputProvider; } }

    IGameStateReadable _currentGameState;
    public IGameStateReadable CurrentGameState { get { return _currentGameState; } }

    Subject<Unit> _getStarSubject = new Subject<Unit> ();
    public IObservable<Unit> GetStarSubject { get { return _getStarSubject; } }

    ReactiveProperty<bool> _isDead = new ReactiveProperty<bool> (false);
    public IReadOnlyReactiveProperty<bool> IsDead { get { return _isDead; } }

    ReactiveProperty<int> _playerLife = new ReactiveProperty<int> (3);
    public IReadOnlyReactiveProperty<int> PlayerLife { get { return _playerLife; } }

    void Awake () {
        _inputProvider = GetComponent<IInputProvider> ();
    }

    public void Initialize (IGameStateReadable gameState) {
        _currentGameState = gameState;

        this.OnTriggerEnter2DAsObservable ()
            .Select (other => other.gameObject.GetComponent<StarItem> ())
            .Where (star => star != null)
            .Subscribe (star => {
                _getStarSubject.OnNext (Unit.Default);
                star.GetStar ();
                AudioManager.Instance.PlaySE (SE.Item.ToString ());
            }).AddTo (this.gameObject);

        PlayerLife.SkipLatestValueOnSubscribe ().Subscribe (x => {
            if (x <= 0) {
                PlayerDead ();
                return;
            }

            // ダメージアニメーション
            var sequence = GetComponent<SpriteRenderer> ().DOFade (0.0f, 0.3f).SetEase (Ease.Linear).SetLoops (3, LoopType.Restart);
        });

        // その他のクラスの初期化
        _initializeSubject.OnNext (Unit.Default);
        _initializeSubject.OnCompleted ();
    }

    public void ApplyDamage (int power) {
        AudioManager.Instance.PlaySE (SE.PlayerHit.ToString ());
        _playerLife.Value -= power;
    }

    void PlayerDead () {
        AudioManager.Instance.PlaySE (SE.LastHit.ToString ());
        Instantiate (_destroyExplosion, transform.position, Quaternion.identity);
        _isDead.Value = true;
    }

    public void CreaPlayer () {
        Destroy (this.gameObject);
    }
}