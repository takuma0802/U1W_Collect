using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerCore : MonoBehaviour
{
    IInputProvider _inputProvider;
    float speed = 0.3f;


    void Start()
    {
        _inputProvider = GetComponent<IInputProvider>();

        _inputProvider.MoveDirection
            .Subscribe(v2 => 
            {
                var hori = v2.x * speed;
                var vert = v2.y * speed;
                transform.Translate(new Vector2(hori,vert));
            });
        
        _inputProvider.AttackButtonDown
            .Where(x => x)
            .Subscribe(x => Debug.Log("ButtonDown！"));
        
        _inputProvider.AttackButtonUp
            .Where(x => x)
            .Subscribe(x => naichilab.RankingLoader.Instance.SendScoreAndShowRanking (100));
    }
}
