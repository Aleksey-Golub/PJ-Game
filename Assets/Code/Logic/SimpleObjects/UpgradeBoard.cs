using Code.Infrastructure;
using Code.Services;
using Code.UI.Services;
using System.Linq;

internal class UpgradeBoard : SimpleObject
{
    private IUIMediator _uiMediator;
    private IConfigsService _configService;
    private IPersistentProgressService _progressService;
    private Inventory _inventory;

    internal bool IsVisited { get; private set; }

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var configService = AllServices.Container.Single<IConfigsService>();
            var progressService = AllServices.Container.Single<IPersistentProgressService>();
            var uiMediator = AllServices.Container.Single<IUIMediator>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();

            Construct(uiMediator, configService, progressService);
            gameFactory.RegisterProgressWatchers(gameObject);
        }
    }

    internal void Construct(IUIMediator uiMediator, IConfigsService configService, IPersistentProgressService progressService)
    {
        _uiMediator = uiMediator;
        _configService = configService;
        _progressService = progressService;
    }

    internal void Open(Inventory inventory)
    {
        Logger.Log($"[UpgradeBoard] Opened");
        IsVisited = true;

        _inventory = inventory;
        _uiMediator.OpenUpgradeBoardView(UpgradeItem);
    }

    internal void Close()
    {
        Logger.Log($"[UpgradeBoard] Closed");
        IsVisited = false;

        _inventory = null;

        _uiMediator.CloseUpgradeBoardView();
    }

    private void UpgradeItem(string itemId)
    {
        _progressService.Progress.PlayerProgress.UpgradeItemsProgress.TryGet(itemId, out int value);
        int nextLevel = value + 1;
        var cost = _configService.UpgradablesConfigs.First(u => u.ID == itemId).GetUpgradeData(nextLevel).Cost;

        if (_inventory.Has(ResourceType.COIN, cost))
        {
            Logger.Log($"[UpgradeBoard] {itemId} upgrading");

            _inventory.Remove(ResourceType.COIN, cost);
            _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Upgrade(itemId);

            _uiMediator.RefreshUpgradeBoardView();
        }
    }
}
