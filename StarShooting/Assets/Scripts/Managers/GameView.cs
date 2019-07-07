using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameView : MonoBehaviour
{
    [SerializeField] GameObject[] _lifeImageObject;
    [SerializeField] Text _timeText, _enemyText, _starText;

    [SerializeField] GameObject ResultUI;
    [SerializeField] Transform ResultUIPosition;
    [SerializeField] Text _scoreResult, _timeResult, _enemyResult,_allEnemy, _starResult;

    public void ResetView()
    {
        ResultUI.transform.DOMoveY(ResultUIPosition.localPosition.y,1f);
        SettingLifeView();
        UpdateTimeView(0);
        UpdateEnemyView(0);
        UpdateStarView(0);
    }

    void SettingLifeView()
    {
        foreach(var image in _lifeImageObject)
        {
            image.transform.localScale = Vector2.zero;
        }
        StartCoroutine(SettingLifeAnimation());
    }

    IEnumerator SettingLifeAnimation()
    {
        foreach(var image in _lifeImageObject)
        {
            var moveSequence = image.transform.DOScale(Vector2.one,0.5f);
            yield return moveSequence.WaitForCompletion();
        }  
    }

    public void CutLifeView(int currentLife)
    {
        _lifeImageObject[currentLife].transform.DOScale(Vector2.zero,0.5f);
    }

    public void UpdateTimeView(float time)
    {
        _timeText.text = time.ToString();
    }

    public void UpdateEnemyView(int num)
    {
        _enemyText.text = num.ToString();
    }

    public void UpdateStarView(int num)
    {
        _starText.text = num.ToString();
    }

    public void ShowResult(int score,int time, int enemy,int allEnemy,int star)
    {
        StartCoroutine(ShowResultCoroutine(score,time,enemy,allEnemy,star));
    }

    IEnumerator ShowResultCoroutine(int score,int time, int enemy,int allEnemy,int star)
    {
        yield return new WaitForSeconds(0.5f);
        var sequence = ResultUI.transform.DOMoveY(0,3f).SetEase(Ease.InBounce);
        yield return sequence.WaitForCompletion();

        var sequence2 = _timeResult.DOTextInt(0, time, 0.4f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        yield return sequence2.WaitForCompletion();

        sequence2 = _enemyResult.DOTextInt(0, enemy, 0.4f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        var sequence3 = _allEnemy.DOTextInt(0, allEnemy, 0.4f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        yield return sequence3.WaitForCompletion();
        
        sequence2 = _starResult.DOTextInt(0, star, 0.4f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        yield return sequence2.WaitForCompletion();

        sequence2 = _scoreResult.DOTextInt(0, score, 1f, it => string.Format("{0:0}", it)).SetEase(Ease.InBounce);
        yield return sequence2.WaitForCompletion();
    }
}
