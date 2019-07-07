using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class StarGenerator : MonoBehaviour
{
    [SerializeField] GameObject _starItemPrefab;
    [SerializeField] float[] StarFrequencyMap = new float[] { };
    [SerializeField] int[] MaxStarNumMap = new int[] { };

    ReactiveCollection<StarItem> _allGeneratedStars = new ReactiveCollection<StarItem>();
    public IReadOnlyReactiveCollection<StarItem> AllGeneratedStars { get { return _allGeneratedStars; } }

    ReactiveProperty<int> _getStarNumber = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> GetStarNumber { get { return _getStarNumber; } }

    CompositeDisposable _compositeDisposable = new CompositeDisposable(); // 生成した敵の死亡通知監視リスト
    IWaveStateReadable _currentWave;
    PlayerCore _core;
    IDisposable _generateStarStream;

    public void Initialize(IWaveStateReadable wave, PlayerCore core)
    {
        _currentWave = wave;
        _core = core;
        CreaAllEnemyStream();
        ManagementEnemyGenerating();

        _generateStarStream = ReSettingGenerateStream();

        _currentWave.CurrentWaveState.Subscribe(x =>
        {
            _generateStarStream = ReSettingGenerateStream();
        });
    }
    IDisposable ReSettingGenerateStream()
    {
        if (_generateStarStream != null) _generateStarStream.Dispose();

        return Observable.Interval(TimeSpan.FromSeconds(StarFrequencyMap[(int)_currentWave.CurrentWaveState.Value]))
                .Where(_ => _allGeneratedStars.Count < MaxStarNumMap[(int)_currentWave.CurrentWaveState.Value])
                .Subscribe(l =>
                {
                    ManagementEnemyGenerating();
                }).AddTo(this);
    }


    void ManagementEnemyGenerating()
    {
        InstantiateEnemy();
    }

    void InstantiateEnemy()
    {
        var limitPos = Utilities.m_moveLimit;
        var xpos = UnityEngine.Random.Range(-limitPos.x + 0.5f,limitPos.x - 0.5f);
        var ypos = UnityEngine.Random.Range(-limitPos.y + 0.5f,limitPos.y - 0.5f);
        var spownPos = new Vector2(xpos,ypos);
        
        var newStar = Instantiate(_starItemPrefab, spownPos, Quaternion.identity).GetComponent<StarItem>();
        newStar.Init();
        _allGeneratedStars.Add(newStar);

        var stream = newStar.IsGet.Where(x => x)
            .Subscribe(_ =>
            {
                _getStarNumber.Value++;
                _allGeneratedStars.Remove(newStar);
            }).AddTo(newStar.gameObject);
        _compositeDisposable.Add(stream);
    }

    public void CreaAllEnemyStream()
    {
        foreach(var enemy in _allGeneratedStars)
        {
            if(enemy.gameObject) Destroy(enemy.gameObject);
        }
        if (_generateStarStream != null) _generateStarStream.Dispose();
        _getStarNumber.Value = 0;
        _allGeneratedStars.Clear();
        _compositeDisposable.Clear();
    }
}
