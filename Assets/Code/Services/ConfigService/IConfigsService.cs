using Code.UI;
using Code.UI.Services;
using System.Collections.Generic;

namespace Code.Services
{
    public interface IConfigsService : IService
    {
        IReadOnlyDictionary<ResourceType, ResourceConfig> ResourcesConfigs { get; }
        IReadOnlyDictionary<ToolType, ToolConfig> ToolsConfigs { get; }
        IReadOnlyDictionary<ResourceStorageType, ResourceStorageConfig> ResourceStorageConfigs { get; }
        IReadOnlyDictionary<ConverterType, ConverterConfig> ConverterConfigs { get; }
        IReadOnlyList<IUpgradable> UpgradablesConfigs { get; }
        IReadOnlyDictionary<EffectId, EffectConfig> EffectsConfigs { get; }

        void Load();
        ResourceConfig GetConfigFor(ResourceType type);
        ToolConfig GetConfigFor(ToolType type);
        WindowConfig GetConfigFor(WindowId windowId);
        EffectConfig GetConfigFor(EffectId effectType);
    }
}