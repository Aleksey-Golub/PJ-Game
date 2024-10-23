﻿using Assets.Code.UI;
using System;
using UnityEngine;

internal class UIService : MonoSingleton<UIService>
{
    [SerializeField] private PlayerInventoryView _playerInventoryView;
    [SerializeField] private SellBoardView _sellBoardView;
    [SerializeField] private UpgradeBoardView _upgradeBoardView;

    internal IInventoryView GetPlayerInventoryView()
    {
        var configService = ConfigsService.Instance;
        _playerInventoryView.Coustruct(configService);

        return _playerInventoryView;
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
}
