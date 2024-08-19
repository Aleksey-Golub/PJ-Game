interface IResourceConsumer
{
    bool CanInteract { get; }
    ResourceConsumerNeeds GetNeeds();
    void Consume(int value);
}

public struct ResourceConsumerNeeds
{
    public ResourceType ResourceType;
    public int CurrentNeedResourceCount;
}
