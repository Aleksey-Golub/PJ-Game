using System.Collections.Generic;

internal interface IInventoryView
{
    void Init(IReadOnlyDictionary<ResourceType, int> storage);
    void UpdateFor(ResourceType resourceType, int newCount);
}
