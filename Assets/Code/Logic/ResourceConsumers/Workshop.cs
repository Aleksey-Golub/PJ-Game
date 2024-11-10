using Code.Services;
using UnityEngine;

[SelectionBase]
public class Workshop : MonoBehaviour, IResourceConsumer
{
    [SerializeField] private ResourceConsumerView _view;
    [SerializeField] private Collider2D _collider;

    [SerializeField] private ResourceConfig _needResourceConfig;
    [SerializeField] private int _needResourceCount = 1;
    [SerializeField] private int _preferedConsumedValue = -1;
    [SerializeField] private Transform _transitionalResourceFinal;

    [SerializeField] private GameObject _spawnObject;

    private IExhaustStrategy _exhaust;
    private int _currentNeedResourceCount;
    private int _currentPreUpload;

    public bool CanInteract => _currentNeedResourceCount != 0 && _currentPreUpload < _needResourceCount;
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

    internal void Init()
    {
        _currentNeedResourceCount = _needResourceCount;
        _currentPreUpload = 0;

        _view.Init(_needResourceConfig.Sprite, _currentNeedResourceCount, null);
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
        _view.ShowNeeds(_currentNeedResourceCount);

        if (_currentNeedResourceCount == 0)
        {
            _view.ShowHitAnimation();
            DropObject();
            Exhaust();
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

        Instantiate(_spawnObject, transform.position, Quaternion.identity);
    }

    private void Exhaust()
    {
        _view.ShowExhaust();

        _exhaust.ExhaustDelayed(1f);
    }
}
