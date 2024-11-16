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

    protected int CurrentNeedResourceCount;
    protected IExhaustStrategy ExhaustStrategy;
    protected int CurrentPreUpload;

    public bool CanInteract => CurrentNeedResourceCount != 0 && CurrentPreUpload < _needResourceCount;
    public int PreferedConsumedValue => _preferedConsumedValue;
    public int FreeSpace => _needResourceCount - CurrentPreUpload;

    public Vector3 TransitionalResourceFinalPosition => _transitionalResourceFinal.position;

    protected string Id => UniqueId.Id;

    protected void Construct()
    {
        ExhaustStrategy = new ExhaustStrategy(this, _collider);
    }

    public abstract void WriteToProgress(GameProgress progress);
    public abstract void ReadProgress(GameProgress progress);

    public void ApplyPreUpload(int consumedValue)
    {
        CurrentPreUpload += consumedValue;
    }

    public void Consume(int value)
    {
        CurrentNeedResourceCount -= value;
        View.ShowNeeds(CurrentNeedResourceCount, _needResourceCount);

        if (CurrentNeedResourceCount == 0)
        {
            OnFilled();
        }
    }

    public ResourceConsumerNeeds GetNeeds()
    {
        return new ResourceConsumerNeeds()
        {
            ResourceType = _needResourceConfig.Type,
            CurrentNeedResourceCount = CurrentNeedResourceCount
        };
    }

    public virtual void Init()
    {
        CurrentNeedResourceCount = _needResourceCount;
        CurrentPreUpload = 0;

        View.Init(_needResourceConfig.Sprite, CurrentNeedResourceCount, GetGenerateObjSprite());
        View.ShowNeeds(CurrentNeedResourceCount, _needResourceCount);
    }

    protected abstract Sprite GetGenerateObjSprite();
    protected abstract void DropObject();

    protected virtual void OnFilled()
    {
        View.ShowHitAnimation();
        DropObject();
        Exhaust();
    }

    protected virtual bool HasChangesBetweenSavedStateAndCurrentState(SingleUseConsumerBaseOnScene data)
    {
        return
            data.CurrentNeedResourceCount != CurrentNeedResourceCount ||
            data.Position.AsUnityVector() != transform.position
            ;
    }

    private void Exhaust()
    {
        View.ShowExhaust();

        ExhaustStrategy.ExhaustDelayed(1f);
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => Accept(visitor);
    protected abstract void Accept(ICreatedByIdGameObjectVisitor visitor);
}
