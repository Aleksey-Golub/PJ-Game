using Code.Infrastructure;
using Code.Services;
using Code.UI.Services;

public class SellBoard : SimpleObjectBase, ICreatedByIdGameObject
{
    private IUIMediator _uiMediator;
    private IConfigsService _configService;
    private Inventory _inventory;

    internal bool IsVisited { get; private set; }
    protected override SimpleObjectType Type => SimpleObjectType.SellBoard;

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var configService = AllServices.Container.Single<IConfigsService>();
            var uiMediator = AllServices.Container.Single<IUIMediator>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();

            Construct(uiMediator, configService);
            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    internal void Construct(IUIMediator uiMediator, IConfigsService configService)
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

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
