using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Devil : BaseEnemy
{
    void Start()
    {
        currentPos = transform.position;
    }

    Vector2 CulcNextTargetPosition(Vector2 currentPos)
    {
        var tagetPos = Vector2.zero;
        return targetPos;
    }
    
    public override void ApplyDamage(int power)
    {

    }

    public void Shoot()
    {
        
    }
}
