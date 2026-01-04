using System.Collections.Generic;
using UnityEngine;

interface IResourceConsumer
{
    bool CanInteract { get; }
    bool Available { get; }

    /// <summary>
    /// Ignored if less then 1
    /// </summary>
    int GetPreferedConsumedValue(ResourceType resourceType);
    int GetFreeSpace(ResourceType resourceType);
    Vector3 GetTransitionalResourceFinalPosition(ResourceType resourceType);
    List<ResourceConsumerNeeds> GetNeeds();
    void Consume(ResourceType resourceType, int value);
    void ApplyPreUpload(ResourceType resourceType, int consumedValue);
    void SetAvailable();
}

public class ResourceConsumerNeeds
{
    public ResourceType ResourceType;
    public int CurrentNeedResourceCount;

    public ResourceConsumerNeeds(ResourceType resourceType)
    {
        ResourceType = resourceType;
    }
}

/// <summary>
/// ResourceConsumer need settings to configurate from inspector
/// </summary>
[System.Serializable]
public class ResourceConsumerNeedSettings
{
    [field: SerializeField] public int NeedIndex { get; private set; } = -1;
    [field: SerializeField] public ResourceConfig NeedResourceConfig { get; private set; }
    [field: SerializeField] public int SingleUpload { get; private set; } = 5;
    [Tooltip("Used when config is null")]
    [field: SerializeField] public int MaxUpload { get; private set; } = 25;
    [field: SerializeField] public int PreferedConsumedValue { get; private set; } = -1;
    [field: SerializeField] public Transform TransitionalResourceFinal { get; private set; }
}

/// <summary>
/// ResourceConsumer runtime need data
/// </summary>
public class ResourceConsumerNeedData
{
    public ResourceConsumerNeedSettings Settings;
    public int CurrentUpload;
    public int CurrentPreUpload;

    public ResourceConsumerNeedData(ResourceConsumerNeedSettings settings)
    {
        Settings = settings;
    }
}
