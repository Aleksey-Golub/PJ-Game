using Code.Services;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Chunk : MonoBehaviour, IResourceConsumer
{
    [SerializeField] private ChunkView _view;
    [SerializeField] private Collider2D _collider;

    [Space()]
    [SerializeField] private bool _openByOtherOnly;
    [SerializeField] private ResourceConfig _needResourceConfig;
    [SerializeField] private int _needResourceCount = 1;
    [SerializeField] private int _preferedConsumedValue = -1;
    [SerializeField] private Transform _transitionalResourceFinal;

    [Space()]
    [SerializeField] private SpawnData[] _spawnDatas;
    [SerializeField] private Chunk[] _chunksToOpen;
    [SerializeField] private float _openDelay = 0.5f;

    private IExhaustStrategy _exhaust;
    private int _currentNeedResourceCount;
    private int _currentPreUpload;

    public bool CanInteract => !_openByOtherOnly && _currentNeedResourceCount != 0 && _currentPreUpload < _needResourceCount;
    public int PreferedConsumedValue => _preferedConsumedValue;
    public int FreeSpace => _needResourceCount - _currentPreUpload;

    public Vector3 TransitionalResourceFinalPosition => _transitionalResourceFinal.position;

    private void Start()
    {
        var audio = AllServices.Container.Single<IAudioService>();
        var effectFactory = AllServices.Container.Single<IEffectFactory>();

        Construct(audio, effectFactory);
        Init();
    }

    private void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        _exhaust = new ExhaustStrategy(this, _collider);

        _view.Construct(audio, effectFactory);
    }

    private void Init()
    {
        _currentNeedResourceCount = _needResourceCount;
        _currentPreUpload = 0;

        _view.Init(_needResourceConfig.Sprite, _currentNeedResourceCount, null);
        _view.ShowNeeds(_currentNeedResourceCount, _needResourceCount);
    }

    public ResourceConsumerNeeds GetNeeds()
    {
        return new ResourceConsumerNeeds()
        {
            ResourceType = _needResourceConfig.Type,
            CurrentNeedResourceCount = _currentNeedResourceCount
        };
    }

    public void Consume(int value)
    {
        _currentNeedResourceCount -= value;
        _view.ShowNeeds(_currentNeedResourceCount, _needResourceCount);

        if (_currentNeedResourceCount == 0)
        {
            Open();
        }
    }
    public void ApplyPreUpload(int consumedValue)
    {
        _currentPreUpload += consumedValue;
    }

    private void DropObject()
    {
        _view.PlayDropResourceSound();
        _view.ShowHitEffect();

        foreach (SpawnData data in _spawnDatas)
            Instantiate(data.Prefab, data.Point.position, Quaternion.identity);
    }

    private IEnumerator OpenDelayed()
    {
        yield return new WaitForSeconds(_openDelay);

        Open();
    }

    private void Open()
    {
        _view.ShowHitAnimation();
        DropObject();
        OpenChainedChunks();
        Exhaust();
    }

    private void OpenChainedChunks()
    {
        foreach (Chunk chunk in _chunksToOpen)
            chunk.StartCoroutine(chunk.OpenDelayed());
    }

    private void Exhaust()
    {
        _view.ShowExhaust();

        _exhaust.ExhaustDelayed(1f);
    }

    [System.Serializable]
    public struct SpawnData
    {
        public Transform Point;
        public GameObject Prefab;
    }
}

