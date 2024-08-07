using System.Collections.Generic;
using UnityEngine;

internal class ResourceConfigService : MonoSingleton<ResourceConfigService>
{
    [SerializeField] private List<ResourceConfig> _configs;

    private readonly Dictionary<ResourceType, ResourceConfig> _cashedConfigs = new();

    protected override void Awake()
    {
        base.Awake();

        CashConfigs();
    }

    internal ResourceConfig GetConfigFor(ResourceType type)
    {
        return _cashedConfigs[type];
    }

    private void CashConfigs()
    {
        foreach (var config in _configs)
        {
            _cashedConfigs.Add(config.Type, config);
        }
    }
}
