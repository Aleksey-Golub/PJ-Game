using Assets.Code.UI;
using UnityEngine;

internal class UIService : MonoSingleton<UIService>
{
    [SerializeField] private PlayerInventoryView _playerInventoryView;

    internal IInventoryView GetPlayerInventoryView()
    {
        var configService = ConfigsService.Instance;
        _playerInventoryView.Coustruct(configService);

        return _playerInventoryView;
    }
}
