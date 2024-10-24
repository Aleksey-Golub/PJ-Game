using UnityEngine;

interface IResourceConsumer
{
    bool CanInteract { get; }
    
    /// <summary>
    /// Ignored if less then 1
    /// </summary>
    int PreferedConsumedValue { get; }
    int FreeSpace { get; }
    Vector3 TransitionalResourceFinalPosition { get; }

    ResourceConsumerNeeds GetNeeds();
    void Consume(int value);
    void ApplyPreUpload(int consumedValue);
}

public struct ResourceConsumerNeeds
{
    public ResourceType ResourceType;
    public int CurrentNeedResourceCount;
}
