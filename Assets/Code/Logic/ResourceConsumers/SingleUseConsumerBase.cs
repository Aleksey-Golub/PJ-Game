using Code.Data;
using Code.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class SingleUseConsumerBase<T> : MonoBehaviour, IResourceConsumer, ISavedProgressReader, ISavedProgressWriter, IUniqueIdHolder, IPossibleSceneBuiltInItem, ICreatedByIdGameObject where T : ResourceConsumerView
{
    [SerializeField] private bool _sendAnalyticEventOnFilled;
    [SerializeField] private Metrika.Event _onFilledAnalyticEvent;

    [field: SerializeField] public bool Available { get; protected set; } = true;
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

    public bool CanInteract => Available && CurrentNeedResourceCount != 0 && CurrentPreUpload < _needResourceCount;

    protected string Id => UniqueId.Id;
    protected Collider2D Collider => _collider;
    protected abstract Action OnExhaustCallback { get; }
    protected abstract bool DisableSelfOnExhaused { get; }

    private List<ResourceConsumerNeeds> _needs;

    protected void Construct()
    {
        _needs = new() { new ResourceConsumerNeeds(_needResourceConfig.Type) };

        ExhaustStrategy = new ExhaustStrategy(this, _collider, OnExhaustCallback, DisableSelfOnExhaused);
    }

    public abstract void WriteToProgress(GameProgress progress);
    public abstract void ReadProgress(GameProgress progress);

    public void ApplyPreUpload(ResourceType resourceType, int consumedValue)
    {
        CurrentPreUpload += consumedValue;
    }

    public void Consume(ResourceType resourceType, int value)
    {
        CurrentNeedResourceCount -= value;
        View.ShowNeeds(CurrentNeedResourceCount, _needResourceCount, Available);

        if (CurrentNeedResourceCount == 0)
        {
            OnFilled();
        }
    }

    public int GetPreferedConsumedValue(ResourceType resourceType) => _preferedConsumedValue;
    public int GetFreeSpace(ResourceType resourceType) => _needResourceCount - CurrentPreUpload;
    public Vector3 GetTransitionalResourceFinalPosition(ResourceType resourceType) => _transitionalResourceFinal.position;

    public List<ResourceConsumerNeeds> GetNeeds()
    {
        _needs[0].CurrentNeedResourceCount = CurrentNeedResourceCount;

        return _needs;
    }

    public void SetAvailable()
    {
        Available = true;
        View.ShowNeeds(CurrentNeedResourceCount, _needResourceCount, Available);
    }

    public virtual void Init()
    {
        CurrentNeedResourceCount = _needResourceCount;
        CurrentPreUpload = 0;

        View.Init(_needResourceConfig.Sprite, CurrentNeedResourceCount, GetGenerateObjSprite());
        View.ShowNeeds(CurrentNeedResourceCount, _needResourceCount, Available);
    }

    protected abstract Sprite GetGenerateObjSprite();
    protected abstract void DropObject();

    protected virtual void OnFilled()
    {
        View.ShowHitAnimation();
        DropObject();
        Exhaust();

        if (_sendAnalyticEventOnFilled)
            Metrika.EventReached(_onFilledAnalyticEvent);
    }

    protected virtual bool HasChangesBetweenSavedStateAndCurrentState(SingleUseConsumerBaseOnScene data)
    {
        return
            data.IsAvailable != Available ||
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
