using Code.Data;
using Code.Services;
using System;
using System.Collections.Generic;

internal class Inventory : ISavedProgressReader, ISavedProgressWriter
{
    private Dictionary<ResourceType, int> _storage;
    private List<ToolType> _tools;

    internal event Action<ResourceType, int> ResourceCountChanged;

    internal IReadOnlyDictionary<ResourceType, int> Storage => _storage;

    internal Inventory()
    {
        _storage = new();
        _tools = new();
    }

    public void WriteToProgress(GameProgress progress)
    {
        progress.PlayerProgress.InventoryData.Tools = _tools;
        progress.PlayerProgress.InventoryData.ResourceStorageData.Dictionary = _storage;
    }

    public void ReadProgress(GameProgress progress)
    {
        _tools = progress.PlayerProgress.InventoryData.Tools;
        _storage = progress.PlayerProgress.InventoryData.ResourceStorageData.Dictionary;
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

    internal bool GetCount(ResourceType type, out int value)
    {
        value = 0;
        if (_storage.TryGetValue(type, out int count))
        {
            value = count;
            return count > 0;
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
