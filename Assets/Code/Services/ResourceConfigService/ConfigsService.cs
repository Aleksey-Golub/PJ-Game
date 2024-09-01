using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class ConfigsService : MonoSingleton<ConfigsService>
{
    [SerializeField] private List<ResourceConfig> _resourcesConfigs;
    [SerializeField] private List<ToolConfig> _toolsConfigs;

    private Dictionary<ResourceType, ResourceConfig> _cashedResourcesConfigs;
    private Dictionary<ToolType, ToolConfig> _cashedToolsConfigs;

    internal IReadOnlyDictionary<ResourceType, ResourceConfig> ResourcesConfigs => _cashedResourcesConfigs;
    internal IReadOnlyDictionary<ToolType, ToolConfig> ToolsConfigs => _cashedToolsConfigs;

    protected override void Awake()
    {
        base.Awake();

        _cashedResourcesConfigs = _resourcesConfigs.ToDictionary(c => c.Type, c => c);
        _cashedToolsConfigs = _toolsConfigs.ToDictionary(c => c.Type, c => c);
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
