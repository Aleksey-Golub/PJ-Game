using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class ConfigsService : MonoSingleton<ConfigsService>
{
    [SerializeField] private List<ResourceConfig> _resourcesConfigs;
    [SerializeField] private List<ToolConfig> _toolsConfigs;
    [SerializeField] private List<ResourceStorageConfig> _resourceStorageConfigs;

    private Dictionary<ResourceType, ResourceConfig> _cashedResourcesConfigs;
    private Dictionary<ToolType, ToolConfig> _cashedToolsConfigs;
    private Dictionary<ResourceStorageType, ResourceStorageConfig> _cashedResourceStorageConfigs;
    private IReadOnlyList<IUpgradable> _cashedUpgradablesConfigs;

    internal IReadOnlyDictionary<ResourceType, ResourceConfig> ResourcesConfigs => _cashedResourcesConfigs;
    internal IReadOnlyDictionary<ToolType, ToolConfig> ToolsConfigs => _cashedToolsConfigs;
    internal IReadOnlyDictionary<ResourceStorageType, ResourceStorageConfig> ResourceStorageConfigs => _cashedResourceStorageConfigs;
    internal IReadOnlyList<IUpgradable> UpgradablesConfigs => _cashedUpgradablesConfigs;

    protected override void Awake()
    {
        base.Awake();

        _cashedResourcesConfigs = _resourcesConfigs.ToDictionary(c => c.Type, c => c);

        _cashedToolsConfigs = _toolsConfigs.ToDictionary(c => c.Type, c => c);
        _cashedResourceStorageConfigs = _resourceStorageConfigs.ToDictionary(c => c.Type, c => c);

        var list = new List<IUpgradable>(_toolsConfigs.Count + _resourceStorageConfigs.Count);
        list.AddRange(_toolsConfigs);
        list.AddRange(_resourceStorageConfigs);
        _cashedUpgradablesConfigs = list;
    }

    internal ResourceConfig GetConfigFor(ResourceType type)
    {
        return _cashedResourcesConfigs[type];
    }
    
    internal ToolConfig GetConfigFor(ToolType type)
    {
        return _cashedToolsConfigs[type];
    }
}
