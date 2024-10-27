using System.Collections.Generic;

namespace Code.UI
{
    public interface IInventoryView
    {
        void Init(IReadOnlyDictionary<ResourceType, int> storage);
        void UpdateFor(ResourceType resourceType, int newCount);
    }
}