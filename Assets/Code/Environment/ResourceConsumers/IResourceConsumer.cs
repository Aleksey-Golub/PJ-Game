interface IResourceConsumer
{
    bool CanInteract { get; }
    
    /// <summary>
    /// Ignored if less then 1
    /// </summary>
    int PreferedConsumedValue { get; }
    int FreeSpace { get; }

    ResourceConsumerNeeds GetNeeds();
    void Consume(int value);
}

public struct ResourceConsumerNeeds
{
    public ResourceType ResourceType;
    public int CurrentNeedResourceCount;
}
