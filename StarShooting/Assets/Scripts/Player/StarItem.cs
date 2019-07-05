using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarItem : MonoBehaviour
{
    public void DestroyStar()
    {
        Destroy(this.gameObject);
    }
}
