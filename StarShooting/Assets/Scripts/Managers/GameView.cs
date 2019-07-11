using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameView : MonoBehaviour
{
    [SerializeField] GameObject[] _lifeImageObject;
    [SerializeField] Text _timeText, _enemyText, _starText, _waveText;

    [SerializeField] GameObject ResultUI;
    [SerializeField] Transform ResultUIPosition;
    [SerializeField] Text _scoreResult, _timeResult, _timeScore, _enemyResult, _enemyScore, _allEnemy, _starResult, _starScore;

    public void ResetView()
    {
        ResultUI.SetActive(false);
        ResultUI.transform.DOMoveY(ResultUIPosition.localPosition.y, 1f);
        SettingLifeView();
        UpdateTimeView(0);
        UpdateEnemyView(0);
        UpdateStarView(0);
        UpdateWaveView(WaveState.Wave0);
        ResetResultView();
    }

    void SettingLifeView()
    {
        foreach (var image in _lifeImageObject)
        {
            image.transform.localScale = Vector2.zero;
        }
        StartCoroutine(SettingLifeAnimation());
    }

    IEnumerator SettingLifeAnimation()
    {
        foreach (var image in _lifeImageObject)
        {
            var moveSequence = image.transform.DOScale(Vector2.one, 0.5f);
            yield return moveSequence.WaitForCompletion();
        }
    }

    public void CutLifeView(int currentLife)
    {
        _lifeImageObject[currentLife].transform.DOScale(Vector2.zero, 0.5f);
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

    public void UpdateWaveView(WaveState wave)
    {
        _waveText.text = wave.ToString();
    }

    void ResetResultView()
    {
        _scoreResult.text = "";
        _timeResult.text = "";
        _timeScore.text = "";
        _enemyResult.text = "";
        _enemyScore.text = "";
        _allEnemy.text = "";
        _starResult.text = "";
        _starScore.text = "";
    }

    public void ShowResult(int[] score, int time, int enemy, int allEnemy, int star)
    {
        StartCoroutine(ShowResultCoroutine(score, time, enemy, allEnemy, star));
    }

    IEnumerator ShowResultCoroutine(int[] score, int time, int enemy, int allEnemy, int star)
    {
        ResultUI.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        // 移動
        var sequence = ResultUI.transform.DOMoveY(0, 3f).SetEase(Ease.InBounce);
        yield return sequence.WaitForCompletion();
        yield return new WaitForSeconds(1f);

        AudioManager.Instance.PlaySE(SE.Number.ToString());

        // time評価
        var sequence2 = _timeResult.DOTextInt(0, time, 0.3f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        yield return sequence2.WaitForCompletion();

        sequence2 = _timeScore.DOTextInt(0, score[0], 0.5f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        yield return sequence2.WaitForCompletion();
        yield return new WaitForSeconds(0.3f);

        // enemy評価
        sequence2 = _enemyResult.DOTextInt(0, enemy, 0.3f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        var sequence3 = _allEnemy.DOTextInt(0, allEnemy, 0.3f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        yield return sequence3.WaitForCompletion();

        sequence2 = _enemyScore.DOTextInt(0, score[1], 0.5f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        yield return sequence2.WaitForCompletion();
        yield return new WaitForSeconds(0.3f);

        // star評価
        sequence2 = _starResult.DOTextInt(0, star, 0.3f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        yield return sequence2.WaitForCompletion();

        sequence2 = _starScore.DOTextInt(0, score[2], 0.5f, it => string.Format("{0:0}", it)).SetEase(Ease.Linear);
        yield return sequence2.WaitForCompletion();
        yield return new WaitForSeconds(0.3f);

        sequence2 = _scoreResult.DOTextInt(0, score[3], 1f, it => string.Format("{0:0}", it)).SetEase(Ease.InBounce);
        yield return sequence2.WaitForCompletion();
    }
}
