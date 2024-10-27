using Code.Services;
using UnityEngine;
using Code.UI.Services;

internal class SellBoard : MonoBehaviour
{
    private IUIMediator _uiMediator;
    private IConfigsService _configService;
    private Inventory _inventory;
 
    internal bool IsVisited { get; private set; }

    private void Start()
    {
        var configService = AllServices.Container.Single<IConfigsService>();
        var uiMediator = AllServices.Container.Single<IUIMediator>();

        Construct(uiMediator, configService);
    }

    private void Construct(IUIMediator uiMediator, IConfigsService configService)
    {
        _uiMediator = uiMediator;
        _configService = configService;
    }

    internal void Open(Inventory inventory)
    {
        Logger.Log($"[SellBoard] Opened");
        IsVisited = true;

        _inventory = inventory;
        _uiMediator.OpenSellBoardView(inventory.Storage, SellResource);
    }

    internal void Close()
    {
        Logger.Log($"[SellBoard] Closed");
        IsVisited = false;

        _inventory = null;

        _uiMediator.CloseSellBoardView();
    }

    private void SellResource(ResourceType resourceType)
    {
        _inventory.GetCount(resourceType, out int count);
        _inventory.Remove(resourceType, count);

        int coinsCount = _configService.GetConfigFor(resourceType).Cost * count;
        _inventory.Add(ResourceType.COIN, coinsCount);

        _uiMediator.RefreshSellBoardView(_inventory.Storage);
    }
}
