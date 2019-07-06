using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerMover : MonoBehaviour
{
    PlayerCore core;
    float _moveSpeed = 0.3f;

    void Awake()
    {
        core = GetComponent<PlayerCore>();
        core.InitializeSubject
            .Subscribe(_ =>
            {
                Initialize();
            });
    }

    void Initialize()
    {
        core.InputProvider.MoveDirection
            .Subscribe(v2 =>
            {
                var hori = v2.x * _moveSpeed;
                var vert = v2.y * _moveSpeed;
                var pos = new Vector2(transform.position.x + hori, transform.position.y + vert);
                pos = Utilities.ClampPosition(pos);
                transform.position = pos;
            }).AddTo(this.gameObject);

        // this.LateUpdateAsObservable()
        //     //.Where(_ => core.CurrentGameState.CurrentGameState.Value == GameState.Main)
        //     .Subscribe(_ => 
        //     {
        //         Utilities.ClampPosition(transform.position);
        //     });
    }
}
