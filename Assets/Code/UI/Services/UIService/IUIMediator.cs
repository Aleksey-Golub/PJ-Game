using Code.Infrastructure;
using Code.Services;
using System;
using System.Collections.Generic;

namespace Code.UI.Services
{
    public interface IUIMediator : IService
    {
        IInventoryView GetPlayerInventoryView();
        void Init(Hud hud);
        void Open(WindowId windowId);
        void OpenSellBoardView(IReadOnlyDictionary<ResourceType, int> storage, Action<ResourceType> sellResource);
        void CloseSellBoardView();
        void RefreshSellBoardView(IReadOnlyDictionary<ResourceType, int> storage);
        void OpenUpgradeBoardView(Action<string> upgradeItem, Inventory inventory);
        void CloseUpgradeBoardView();
        void RefreshUpgradeBoardView();
        bool IsOpened(WindowId windowId);
        void Close(WindowId windowId);
        void OpenPopup(UIPopupId uIPopupId);
        void ClosePopup(UIPopupId uIPopupId);
    }
}
