using Assets.Code.UI;
using UnityEngine;

internal class UIService : MonoSingleton<UIService>
{
    [SerializeField] private PlayerInventoryView _playerInventoryView;
    [SerializeField] private SellBoardView _sellBoardView;

    internal IInventoryView GetPlayerInventoryView()
    {
        var configService = ConfigsService.Instance;
        _playerInventoryView.Coustruct(configService);

        return _playerInventoryView;
    }

    internal SellBoardView GetSellBoardView()
    {
        var configService = ConfigsService.Instance;
        _sellBoardView.Coustruct(configService);

        return _sellBoardView;
    }
}
