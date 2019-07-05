using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerMover : MonoBehaviour
{
    PlayerCore core;
    float _moveSpeed = 0.3f;
    
    void Start()
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
                transform.Translate(new Vector2(hori, vert));
            });
    }
}
