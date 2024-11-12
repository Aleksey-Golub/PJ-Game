using UnityEngine;

[SelectionBase]
public abstract class SingleUseConsumerBase<T> : MonoBehaviour, IResourceConsumer where T : ResourceConsumerView
{
    [SerializeField] protected T View;
    [SerializeField] private Collider2D _collider;

    [SerializeField] private ResourceConfig _needResourceConfig;
    [SerializeField] private int _needResourceCount = 1;
    [SerializeField] private int _preferedConsumedValue = -1;
    [SerializeField] private Transform _transitionalResourceFinal;

    private IExhaustStrategy _exhaust;
    private int _currentNeedResourceCount;
    private int _currentPreUpload;

    public bool CanInteract => _currentNeedResourceCount != 0 && _currentPreUpload < _needResourceCount;
    public int PreferedConsumedValue => _preferedConsumedValue;
    public int FreeSpace => _needResourceCount - _currentPreUpload;

    public Vector3 TransitionalResourceFinalPosition => _transitionalResourceFinal.position;

    protected void Construct()
    {
        _exhaust = new ExhaustStrategy(this, _collider);
    }

    public void ApplyPreUpload(int consumedValue)
    {
        _currentPreUpload += consumedValue;
    }

    public void Consume(int value)
    {
        _currentNeedResourceCount -= value;
        View.ShowNeeds(_currentNeedResourceCount, _needResourceCount);

        if (_currentNeedResourceCount == 0)
        {
            OnFilled();
        }
    }

    public ResourceConsumerNeeds GetNeeds()
    {
        return new ResourceConsumerNeeds()
        {
            ResourceType = _needResourceConfig.Type,
            CurrentNeedResourceCount = _currentNeedResourceCount
        };
    }

    protected virtual void Init()
    {
        _currentNeedResourceCount = _needResourceCount;
        _currentPreUpload = 0;

        View.Init(_needResourceConfig.Sprite, _currentNeedResourceCount, GetGenerateObjSprite());
        View.ShowNeeds(_currentNeedResourceCount, _needResourceCount);
    }

    protected abstract Sprite GetGenerateObjSprite();
    protected abstract void DropObject();

    protected virtual void OnFilled()
    {
        View.ShowHitAnimation();
        DropObject();
        Exhaust();
    }

    private void Exhaust()
    {
        View.ShowExhaust();

        _exhaust.ExhaustDelayed(1f);
    }
}
