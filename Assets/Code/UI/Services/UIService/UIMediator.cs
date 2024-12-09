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
        private readonly Dictionary<UIPopupId, UIPopup> _popupsCache = new();

        public UIMediator(IUIFactory uIFactory)
        {
            _uIFactory = uIFactory;
        }

        public void Init(Hud hud)
        {
            _hud = hud;
        }

        public void OpenPopup(UIPopupId uiPopupId)
        {
            if (IsOpened(uiPopupId))
                return;

            if (!_popupsCache.TryGetValue(uiPopupId, out UIPopup popup))
            {
                popup = _uIFactory.CreatePopup(uiPopupId);
                _popupsCache.Add(uiPopupId, popup);
            }

            switch (uiPopupId)
            {
                case UIPopupId.AdWaiting:
                    popup.Open();
                    break;
                case UIPopupId.None:
                default:
                    throw new NotImplementedException($"Not implemented for {uiPopupId}");
            }
        }

        public void ClosePopup(UIPopupId uiPopupId)
        {
            if (!_popupsCache.TryGetValue(uiPopupId, out UIPopup popup))
                return;

            popup.Close();
        }

        public void Open(WindowId windowId)
        {
            if (IsOpened(windowId))
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

        public bool IsOpened(WindowId windowId)
        {
            return _windowsCache.TryGetValue(windowId, out WindowBase window) && window.IsOpened;
        }

        public bool IsOpened(UIPopupId popupId)
        {
            return _popupsCache.TryGetValue(popupId, out UIPopup popup) && popup.IsOpened;
        }

        public void OpenSellBoardView(IReadOnlyDictionary<ResourceType, int> storage, Action<ResourceType> onSellResource) => SellBoardView.Open(storage, onSellResource);
        public void CloseSellBoardView() => SellBoardView.Close();
        public void RefreshSellBoardView(IReadOnlyDictionary<ResourceType, int> storage) => SellBoardView.Refresh(storage);
        public void OpenUpgradeBoardView(Action<string> OnUpgradeItem, Inventory inventory) => UpgradeBoardView.Open(OnUpgradeItem, inventory);
        public void CloseUpgradeBoardView() => UpgradeBoardView.Close();
        public void RefreshUpgradeBoardView() => UpgradeBoardView.Refresh();

        public void Close(WindowId windowId)
        {
            if (_windowsCache.TryGetValue(windowId, out WindowBase window) && window.IsOpened)
                window.Close();
        }
    }

    public enum WindowId
    {
        None = 0,
        Settings = 1,
        //Sell = 2,
        //Upgrade = 3,
    }

    public enum UIPopupId
    {
        None = 0,
        AdWaiting = 1,
    }
}
