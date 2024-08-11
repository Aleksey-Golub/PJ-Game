using System;
using System.Collections.Generic;

internal class Inventory
{
    private readonly Dictionary<ResourceType, int> _storage;
    private readonly List<ToolType> _tools;

    internal event Action<ResourceType, int> ResourceCountChanged;

    internal IReadOnlyDictionary<ResourceType, int> Storage => _storage;

    internal Inventory()
    {
        _storage = new();
        _tools = new();

        var resTypes = Enum.GetValues(typeof(ResourceType));
        foreach (ResourceType type in resTypes)
        {
            if (type is ResourceType.None)
                continue;

            _storage[type] = 0;
        }
    }

    internal void Add(ResourceType type, int value)
    {
        _storage[type] += value;
        ResourceCountChanged?.Invoke(type, _storage[type]);
    }

    internal bool Remove(ResourceType type, int value)
    {
        if (Has(type, value))
        {
            _storage[type] -= value;
            ResourceCountChanged?.Invoke(type, _storage[type]);
            return true;
        }
        else
        {
            return false;
        }
    }

    internal bool Has(ResourceType type, int value)
    {
        if (_storage.TryGetValue(type, out int count))
        {
            return count >= value;
        }

        return false;
    }

    internal void Add(ToolType type)
    {
        _tools.Add(type);
    }

    internal bool Has(ToolType toolType)
    {
        return _tools.Contains(toolType);
    }
}
