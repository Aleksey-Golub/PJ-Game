using System;
using System.Collections.Generic;

internal class DropCountCalculatorService : MonoSingleton<DropCountCalculatorService>
{
    private Dictionary<ResourceType, int> _dropCountStorage;

    private PersistentProgressService _progressService;
    private ConfigsService _configsService;

    private void Start()
    {
        var progressService = PersistentProgressService.Instance;
        var configsService = ConfigsService.Instance;

        Construct(progressService, configsService);
    }

    internal void Construct(PersistentProgressService progressService, ConfigsService configsService)
    {
        _dropCountStorage = new();

        _progressService = progressService;
        _configsService = configsService;
    }

    internal int Calculate(int originCount, ResourceType resourceType, ToolType needToolType)
    {
        ToolConfig toolConfig = _configsService.GetConfigFor(needToolType);
        string toolId = toolConfig.ID;
        int toolUpgradeLevel = _progressService.Progress.PlayerProgress.UpgradeItemsProgress.UpgradeItemsData.Dictionary[toolId];

        if (toolUpgradeLevel <= 0)
        {
            Logger.LogError($"[DropCountCalculatorService] Some strange situation, gather resource without tool?");
            return originCount;
        }

        float gatherEfficiency = toolConfig.GetUpgradeData(toolUpgradeLevel).Value;

        // used 100 to convert into int to save precision
        int gathered = (int)UnityEngine.Mathf.Round(gatherEfficiency * 100 * originCount);
        if (_dropCountStorage.TryGetValue(resourceType, out int storedCount))
        {
            gathered += storedCount;
        }

        int result = gathered / 100;
        _dropCountStorage[resourceType] = gathered - result * 100;

        return result;
    }
}
