using Assets.Code.UI;
using System;
using System.Linq;
using UnityEngine;

internal class UpgradeBoard : MonoBehaviour
{
    private UpgradeBoardView _view;
    private ConfigsService _configService;
    private PersistentProgressService _progressService;

    internal bool IsVisited { get; private set; }

    private Inventory _inventory;

    private void Start()
    {
        var upgradeBoardView = UIService.Instance.GetUpgradeBoardView();
        var configService = ConfigsService.Instance;
        var progressService = PersistentProgressService.Instance;

        Construct(upgradeBoardView, configService, progressService);
    }

    private void Construct(UpgradeBoardView sellBoardView, ConfigsService configService, PersistentProgressService progressService)
    {
        _configService = configService;
        _progressService = progressService;

        _view = sellBoardView;
    }

    internal void Open(Inventory inventory)
    {
        Logger.Log($"[UpgradeBoard] Opened");
        IsVisited = true;

        _inventory = inventory;
        _view.Open(UpgradeItem);
    }

    internal void Close()
    {
        Logger.Log($"[UpgradeBoard] Closed");
        IsVisited = false;

        _inventory = null;

        if (_view.gameObject.activeSelf)
            _view.Close();
    }

    private void UpgradeItem(string itemId)
    {
        int nextLevelIndex = _progressService.Progress.PlayerProgress.UpgradeItemsProgress.UpgradeItemsData.Dictionary[itemId];
        var cost = _configService.ToolsConfigs.First(p => p.Value.ID == itemId).Value.UpgradeStaticDatas[nextLevelIndex].Cost;

        if (_inventory.Has(ResourceType.COIN, cost))
        {
            Logger.Log($"[UpgradeBoard] {itemId} upgrading");
    
            _inventory.Remove(ResourceType.COIN, cost);
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Upgrade(itemId);

            _view.Refresh();
        }
    }
}
