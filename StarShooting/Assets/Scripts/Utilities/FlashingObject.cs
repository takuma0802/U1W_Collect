using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class FlashingObject : MonoBehaviour {
    [SerializeField] private float angularFrequency = 5f;
    private Image image;

    void Start () {
        float time = 0.0f;
        image = GetComponent<Image> ();
        Observable.Interval (TimeSpan.FromSeconds (Time.deltaTime))
            .Where (_ => this.gameObject.activeInHierarchy)
            .Subscribe (_ => {
                time += angularFrequency * Time.deltaTime;
                var color = image.color;
                color.a = Mathf.Sin (time) * 0.5f + 0.5f;
                image.color = color;
            }).AddTo (this);
    }
}