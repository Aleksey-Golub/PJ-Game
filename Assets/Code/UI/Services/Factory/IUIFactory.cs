using Code.Services;

namespace Code.UI.Services
{
    public interface IUIFactory : IService
    {
        void CreateUIRoot();
        WindowBase CreateWindow(WindowId windowId);
        UIPopup CreatePopup(UIPopupId uiPopupId);
        UpgradeBoardView CreateUpgradeBoardView();
        SellBoardView CreateSellBoardView();
    }
}