using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{
    [SerializeField] GameObject _apearMark;
    [SerializeField] GameObject[] EnemyPrefabs;
    [SerializeField] Transform[] SpownPositions;
    [SerializeField] int[] EnemyTypeMap = new int[] { };
    [SerializeField] float[] EnemyFrequencyMap = new float[] { };
    [SerializeField] int[] MaxEnemyNumMap = new int[] { };


    // 現在生きている敵の数
    ReactiveCollection<BaseEnemy> _alivingEnemies = new ReactiveCollection<BaseEnemy>();
    public IReadOnlyReactiveCollection<BaseEnemy> AlivingEnemies { get { return _alivingEnemies; } }

    ReactiveProperty<int> _allGeneratedEnemies = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> AllGeneratedEnemies { get { return _allGeneratedEnemies; } }

    ReactiveProperty<int> _killNumber = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> KillNumber { get { return _killNumber; } }


    CompositeDisposable _compositeDisposable = new CompositeDisposable(); // 生成した敵の死亡通知監視リスト
    IWaveStateReadable _currentWave;
    PlayerCore _core;
    IDisposable _generateEnemyStream;

    public void Initialize(IWaveStateReadable wave, PlayerCore core)
    {
        _currentWave = wave;
        _core = core;
        CreaAllEnemyStream();
        ManagementEnemyGenerating();

        Observable.Timer(TimeSpan.FromSeconds(5))
            .First()
            .Subscribe(_ =>
            {
                _generateEnemyStream = ReSettingGenerateStream();
            });


        _currentWave.CurrentWaveState.Subscribe(x =>
        {
            _generateEnemyStream = ReSettingGenerateStream();
        });
    }

    IDisposable ReSettingGenerateStream()
    {
        if (_generateEnemyStream != null) _generateEnemyStream.Dispose();

        return Observable.Interval(TimeSpan.FromSeconds(EnemyFrequencyMap[(int)_currentWave.CurrentWaveState.Value]))
                .Where(_ => _alivingEnemies.Count < MaxEnemyNumMap[(int)_currentWave.CurrentWaveState.Value])
                .Subscribe(l =>
                {
                    ManagementEnemyGenerating();
                }).AddTo(this);
    }


    void ManagementEnemyGenerating()
    {
        var enemyNum = UnityEngine.Random.Range(0, EnemyTypeMap[(int)_currentWave.CurrentWaveState.Value]);
        var spownNum = UnityEngine.Random.Range(0, SpownPositions.Length);

        StartCoroutine(InstantiateEnemy(enemyNum, spownNum));
    }

    IEnumerator InstantiateEnemy(int type, int spownArea, Vector3 offset = new Vector3())
    {
        var spownPos = SpownPositions[spownArea].position + offset;
        var mark = Instantiate(_apearMark, spownPos, Quaternion.identity);
        var sequence = mark.GetComponent<SpriteRenderer>().DOFade(0.0f, 0.6f).SetEase(Ease.Linear).SetLoops(3, LoopType.Restart);
        yield return sequence.WaitForCompletion();
        Destroy(mark);

        var newEnemy = Instantiate(EnemyPrefabs[type], spownPos, Quaternion.identity).GetComponent<BaseEnemy>();
        var spownType = Utilities.ConvertToEnum<SpownArea>(spownArea);
        newEnemy.Init(spownType, 1f, _core);
        _alivingEnemies.Add(newEnemy);

        var stream = newEnemy.IsDead.Where(x => x > 0)
            .Subscribe(x =>
            {
                if (x == 1) _killNumber.Value++;

                _alivingEnemies.Remove(newEnemy);
            }).AddTo(newEnemy.gameObject);
        _allGeneratedEnemies.Value++;
        _compositeDisposable.Add(stream);
    }

    public void CreaAllEnemyStream()
    {
        foreach (var enemy in _alivingEnemies) Destroy(enemy.gameObject);
        if (_generateEnemyStream != null) _generateEnemyStream.Dispose();
        _killNumber.Value = 0;
        _alivingEnemies.Clear();
        _compositeDisposable.Clear();
    }
}

public enum EnemyType
{
    MarsMan = 0,
    MoonFace = 1,
    OneEye = 2,
    SpaceMan = 3,
    Devil = 4
}

public enum SpownArea
{
    UPPER = 0,
    UPPERLEFT = 1,
    LEFT = 2,
    BOTTOMLEFT = 3,
    BOTTOM = 4,
    BOTTOMRIGHT = 5,
    RIGHT = 6,
    UPPERRIGHT = 7
}
