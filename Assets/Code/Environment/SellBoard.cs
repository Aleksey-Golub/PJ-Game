using Assets.Code.UI;
using UnityEngine;

internal class SellBoard : MonoBehaviour
{
    private SellBoardView _view;
    private ConfigsService _configService;
    private Inventory _inventory;

    internal bool IsVisited { get; private set; }

    private void Start()
    {
        var sellBoardView = UIService.Instance.GetSellBoardView();
        var configService = ConfigsService.Instance;
        Construct(sellBoardView, configService);
    }

    private void Construct(SellBoardView sellBoardView, ConfigsService configService)
    {
        _configService = configService;
        _view = sellBoardView;
    }

    internal void Open(Inventory inventory)
    {
        Logger.Log($"[SellBoard] Opened");
        IsVisited = true;

        _inventory = inventory;
        _view.Open(inventory.Storage, SellResource);
    }

    internal void Close()
    {
        Logger.Log($"[SellBoard] Closed");
        IsVisited = false;

        _inventory = null;

        if (_view.gameObject.activeSelf)
            _view.Close();
    }

    private void SellResource(ResourceType resourceType)
    {
        _inventory.GetCount(resourceType, out int count);
        _inventory.Remove(resourceType, count);

        int coinsCount = _configService.GetConfigFor(resourceType).Cost * count;
        _inventory.Add(ResourceType.COIN, coinsCount);

        _view.Refresh(_inventory.Storage);
    }
}
