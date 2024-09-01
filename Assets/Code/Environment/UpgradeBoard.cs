using Assets.Code.UI;
using System;
using UnityEngine;

internal class UpgradeBoard : MonoBehaviour
{
    private UpgradeBoardView _view;
    private ConfigsService _configService;

    internal bool IsVisited { get; private set; }
    
    private void Start()
    {
        var upgradeBoardView = UIService.Instance.GetUpgradeBoardView();
        var configService = ConfigsService.Instance;
        Construct(upgradeBoardView, configService);
    }

    private void Construct(UpgradeBoardView sellBoardView, ConfigsService configService)
    {
        _configService = configService;
        _view = sellBoardView;
    }

    internal void Open()
    {
        Logger.Log($"[UpgradeBoard] Opened");
        IsVisited = true;

        _view.Open(UpgradeItem);
    }

    internal void Close()
    {
        Logger.Log($"[UpgradeBoard] Closed");
        IsVisited = false;

        if (_view.gameObject.activeSelf)
            _view.Close();
    }

    private void UpgradeItem(ToolType type)
    {
        Logger.Log($"[UpgradeBoard] {type} upgraded");

        //_inventory.GetCount(type, out int count);
        //_inventory.Remove(type, count);

        //int coinsCount = _configService.GetConfigFor(type).Cost * count;
        //_inventory.Add(ResourceType.COIN, coinsCount);

        //_view.Refresh(_inventory.Storage);
    }
}
