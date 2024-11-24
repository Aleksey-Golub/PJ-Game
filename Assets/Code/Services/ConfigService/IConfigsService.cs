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
        IReadOnlyDictionary<string, TutorialMatcher> TutorialsMatchers { get; }

        void Load();
        ResourceConfig GetConfigFor(ResourceType type);
        ToolConfig GetConfigFor(ToolType type);
        EffectConfig GetConfigFor(EffectId effectType);
        WindowMatcher GetMatcherFor(WindowId windowId);
        ResourceSourceMatcher GetMatcherFor(ResourceSourceType type);
        ResourceStorageMatcher GetMatcherFor(ResourceStorageType type);
        SimpleObjectMatcher GetMatcherFor(SimpleObjectType type);
        GameObjectMatcher GetMatcherFor(string gameObjectId);
        TutorialMatcher GetMatcherForTutorial(string sceneName);
    }
}