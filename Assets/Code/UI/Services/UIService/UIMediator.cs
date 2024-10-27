using Code.Infrastructure;
using System;
using System.Collections.Generic;

namespace Code.UI.Services
{
    internal class UIMediator : IUIMediator
    {
        private readonly IUIFactory _uIFactory;

        private Hud _hud;
        private SellBoardView _sellBoardView;
        private UpgradeBoardView _upgradeBoardView;

        private SellBoardView SellBoardView => _sellBoardView ??= _uIFactory.CreateSellBoardView();
        private UpgradeBoardView UpgradeBoardView => _upgradeBoardView ??= _uIFactory.CreateUpgradeBoardView();

        private readonly Dictionary<WindowId, WindowBase> _windowsCache = new();

        public UIMediator(IUIFactory uIFactory)
        {
            _uIFactory = uIFactory;
        }

        public void Init(Hud hud)
        {
            _hud = hud;
        }

        public void Open(WindowId windowId)
        {
            if (IsWindowAlreadyOpened(windowId))
                return;

            if (!_windowsCache.TryGetValue(windowId, out WindowBase window))
            {
                window = _uIFactory.CreateWindow(windowId);
                _windowsCache.Add(windowId, window);
            }

            switch (windowId)
            {
                case WindowId.Settings:
                    ((SettingsWindow)window).Open();
                    break;
                //case WindowId.Sell:
                //case WindowId.Upgrade:
                case WindowId.None:
                default:
                    throw new NotImplementedException($"Not implemented for {windowId}");
            }
        }

        public IInventoryView GetPlayerInventoryView()
        {
            return _hud.PlayerInventoryView;
        }

        private bool IsWindowAlreadyOpened(WindowId windowId)
        {
            return _windowsCache.TryGetValue(windowId, out WindowBase window) && window.IsOpened;
        }

        public void OpenSellBoardView(IReadOnlyDictionary<ResourceType, int> storage, Action<ResourceType> onSellResource) => SellBoardView.Open(storage, onSellResource);
        public void CloseSellBoardView() => SellBoardView.Close();
        public void RefreshSellBoardView(IReadOnlyDictionary<ResourceType, int> storage) => SellBoardView.Refresh(storage);
        public void OpenUpgradeBoardView(Action<string> OnUpgradeItem) => UpgradeBoardView.Open(OnUpgradeItem);
        public void CloseUpgradeBoardView() => UpgradeBoardView.Close();
        public void RefreshUpgradeBoardView() => UpgradeBoardView.Refresh();
    }

    public enum WindowId
    {
        None = 0,
        Settings = 1,
        //Sell = 2,
        //Upgrade = 3,
    }
}
