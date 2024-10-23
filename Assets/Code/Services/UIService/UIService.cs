using Assets.Code.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class UIService : MonoSingleton<UIService>
{
    [SerializeField] private PlayerInventoryView _playerInventoryView;
    [SerializeField] private SellBoardView _sellBoardView;
    [SerializeField] private UpgradeBoardView _upgradeBoardView;
    [SerializeField] private SettingsWindow _settingsWindow;
    [SerializeField] private GameObject _hud;

    private Dictionary<WindowId, WindowBase> _windowsCache;

    protected override void Awake()
    {
        base.Awake();

        _windowsCache = new()
        {
            { WindowId.Settings, _settingsWindow},
        };

        var audio = AudioService.Instance;

        foreach (OpenWindowButton b in _hud.GetComponentsInChildren<OpenWindowButton>())
            b.Construct(this, audio);

        _settingsWindow.Construct(audio);
    }

    internal IInventoryView GetPlayerInventoryView()
    {
        var configService = ConfigsService.Instance;
        _playerInventoryView.Coustruct(configService);

        return _playerInventoryView;
    }

    internal void Open(WindowId windowId)
    {
        if (IsWindowAlreadyOpened(windowId))
            return;

        switch (windowId)
        {
            case WindowId.Settings:
                _settingsWindow.Open();
                break;
            case WindowId.None:
            default:
                throw new NotImplementedException($"Not implemented for {windowId}");
        }
    }

    internal UpgradeBoardView GetUpgradeBoardView()
    {
        var configService = ConfigsService.Instance;
        var progressService = PersistentProgressService.Instance;
        var audio = AudioService.Instance;

        _upgradeBoardView.Coustruct(configService, progressService, audio);

        return _upgradeBoardView;
    }

    internal SellBoardView GetSellBoardView()
    {
        var configService = ConfigsService.Instance;
        var audio = AudioService.Instance;

        _sellBoardView.Coustruct(configService, audio);

        return _sellBoardView;
    }

    private bool IsWindowAlreadyOpened(WindowId windowId)
    {
        return _windowsCache.TryGetValue(windowId, out WindowBase window) && window.IsOpened;
    }
}

public enum WindowId
{
    None = 0,
    Settings = 1,
}
