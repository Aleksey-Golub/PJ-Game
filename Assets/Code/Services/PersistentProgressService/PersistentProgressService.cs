using Assets.Code.Data;

internal class PersistentProgressService : MonoSingleton<PersistentProgressService>
{
    internal GameProgress Progress {get; set;}

#region Not PersistentProgressService, move to Load Progress game State
    // TODO: see region
    private void Start()
    {
        GameProgress loadedProgress = null;
        Progress = loadedProgress ?? NewProgress();

        foreach (var item in Progress.PlayerProgress.UpgradeItemsProgress.UpgradeItemsData.Dictionary)
        {
            Logger.Log($"{item.Key} : {item.Value}");
        }
    }

    private GameProgress NewProgress()
    {
        var configsService = ConfigsService.Instance;

        var progress = new GameProgress();

        // set some data here
        SetInitialUpgradeData(configsService, progress);

        return progress;
    }

    private static void SetInitialUpgradeData(ConfigsService configsService, GameProgress progress)
    {
        var upgradeItemsData = progress.PlayerProgress.UpgradeItemsProgress.UpgradeItemsData;
        int startUpgradeLevel = 0;

        foreach (var toolConfig in configsService.ToolsConfigs)
            if (toolConfig.Value.IsUpgradable)
               upgradeItemsData.Dictionary.Add(toolConfig.Value.ID, startUpgradeLevel);
    }
#endregion
}
