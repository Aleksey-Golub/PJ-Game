using System.Collections.Generic;

namespace Code.Services
{
    internal class DropCountCalculatorService : IDropCountCalculatorService
    {
        private readonly Dictionary<ResourceType, int> _dropCountStorage;

        private readonly IPersistentProgressService _progressService;
        private readonly IConfigsService _configsService;

        public DropCountCalculatorService(IPersistentProgressService progressService, IConfigsService configs)
        {
            _dropCountStorage = new();

            _progressService = progressService;
            _configsService = configs;
        }

        public int Calculate(int originCount, ResourceType resourceType, ToolType needToolType)
        {
            ToolConfig toolConfig = _configsService.GetConfigFor(needToolType);
            string toolId = toolConfig.ID;
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.TryGet(toolId, out int toolUpgradeLevel);

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
}