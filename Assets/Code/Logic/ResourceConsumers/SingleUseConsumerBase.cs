using Code.Data;
using Code.Services;
using UnityEngine;

[SelectionBase]
public abstract class SingleUseConsumerBase<T> : MonoBehaviour, IResourceConsumer, ISavedProgressReader, ISavedProgressWriter, IUniqueIdHolder, IPossibleSceneBuiltInItem, ICreatedByIdGameObject where T : ResourceConsumerView
{
    [field: SerializeField] public bool SceneBuiltInItem { get; private set; }
    [field: SerializeField] public UniqueId UniqueId { get; private set; }
    [SerializeField] protected T View;
    [SerializeField] private Collider2D _collider;

    [SerializeField] protected ResourceConfig _needResourceConfig;
    [SerializeField] protected int _needResourceCount = 1;
    [SerializeField] private int _preferedConsumedValue = -1;
    [SerializeField] private Transform _transitionalResourceFinal;

    protected int _currentNeedResourceCount;
    protected IExhaustStrategy _exhaust;
    protected int _currentPreUpload;

    public bool CanInteract => _currentNeedResourceCount != 0 && _currentPreUpload < _needResourceCount;
    public int PreferedConsumedValue => _preferedConsumedValue;
    public int FreeSpace => _needResourceCount - _currentPreUpload;

    public Vector3 TransitionalResourceFinalPosition => _transitionalResourceFinal.position;

    protected void Construct()
    {
        _exhaust = new ExhaustStrategy(this, _collider);
    }

    public abstract void WriteToProgress(GameProgress progress);
    public abstract void ReadProgress(GameProgress progress);

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

    public virtual void Init()
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

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => Accept(visitor);
    protected abstract void Accept(ICreatedByIdGameObjectVisitor visitor);
}
