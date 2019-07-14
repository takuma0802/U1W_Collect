using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class StarItem : MonoBehaviour {
    [SerializeField] GameObject getAnimation;
    ReactiveProperty<bool> _isGet = new BoolReactiveProperty (false);
    public IReadOnlyReactiveProperty<bool> IsGet { get { return _isGet; } }

    public void Init () {
        StartCoroutine (Apear ());
    }
    IEnumerator Apear () {
        transform.localScale = new Vector2 (0, 0);
        var moveSequence = transform.DOScale (Vector2.one, 1f);
        yield return moveSequence.WaitForCompletion ();
    }

    public void GetStar () {
        Instantiate (getAnimation, transform.position, Quaternion.identity);
        _isGet.Value = true;
        Destroy (this.gameObject);
    }
}