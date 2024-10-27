using Code.UI;
using Code.UI.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Services
{
    internal class ConfigsService : IConfigsService
    {
        private const string RESOURCES_CONFIGS_PATH = "Configs/ResourceConfigs/AllResourcesConfigs";
        private const string TOOLS_CONFIGS_PATH = "Configs/ToolConfigs/AllToolsConfigs";
        private const string RESOURCE_STORAGE_CONFIGS_PATH = "Configs/ResourceStorageConfigs/AllResourceStoragesConfigs";
        private const string CONVERTER_CONFIGS_PATH = "Configs/ConverterConfigs/AllConvertersConfigs";
        private const string WINDOWS_CONFIGS_PATH = "Configs/UI/WindowsConfigs";

        private Dictionary<ResourceType, ResourceConfig> _resourcesConfigs;
        private Dictionary<ToolType, ToolConfig> _toolsConfigs;
        private Dictionary<ResourceStorageType, ResourceStorageConfig> _resourceStorageConfigs;
        private Dictionary<ConverterType, ConverterConfig> _converterConfigs;
        private Dictionary<WindowId, WindowConfig> _windowConfigs;

        public IReadOnlyDictionary<ResourceType, ResourceConfig> ResourcesConfigs => _resourcesConfigs;
        public IReadOnlyDictionary<ToolType, ToolConfig> ToolsConfigs => _toolsConfigs;
        public IReadOnlyDictionary<ResourceStorageType, ResourceStorageConfig> ResourceStorageConfigs => _resourceStorageConfigs;
        public IReadOnlyDictionary<ConverterType, ConverterConfig> ConverterConfigs => _converterConfigs;
        public IReadOnlyList<IUpgradable> UpgradablesConfigs { get; private set; }

        public void Load()
        {
            _resourcesConfigs = Resources.Load<ResourcesConfigs>(RESOURCES_CONFIGS_PATH).Configs.ToDictionary(r => r.Type, r => r);
            _toolsConfigs = Resources.Load<ToolsConfigs>(TOOLS_CONFIGS_PATH).Configs.ToDictionary(c => c.Type, c => c);
            _resourceStorageConfigs = Resources.Load<ResourceStoragesConfigs>(RESOURCE_STORAGE_CONFIGS_PATH).Configs.ToDictionary(c => c.Type, c => c);
            _converterConfigs = Resources.Load<ConvertersConfigs>(CONVERTER_CONFIGS_PATH).Configs.ToDictionary(c => c.Type, c => c);
            _windowConfigs = Resources.Load<WindowsConfigs>(WINDOWS_CONFIGS_PATH).Configs.ToDictionary(c => c.WindowId, c => c);

            UpgradablesConfigs = GetUpgradablesConfigs();
        }

        private List<IUpgradable> GetUpgradablesConfigs()
        {
            List<IUpgradable> list = new List<IUpgradable>(_toolsConfigs.Count + _resourceStorageConfigs.Count + _converterConfigs.Count);
            list.AddRange(_toolsConfigs.Values);
            list.AddRange(_resourceStorageConfigs.Values);
            list.AddRange(_converterConfigs.Values);

            return list;
        }

        public ResourceConfig GetConfigFor(ResourceType type)
        {
            return _resourcesConfigs[type];
        }

        public ToolConfig GetConfigFor(ToolType type)
        {
            return _toolsConfigs[type];
        }

        public WindowConfig GetConfigFor(WindowId windowId)
        {
            return _windowConfigs[windowId];
        }
    }
}