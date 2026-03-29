using Code.Data;
using System;

namespace Code.Services
{
    public interface IIAPService : IService
    {
        public PlayerIAPsData PlayerIAPsData { get; set; }
        event Action<bool> Purchased;

        void PostConstruct(IPersistentProgressService progressService, ISaveLoadService saveService, ISaveLoadIAPDataService saveLoadIAPDataService);
        /*void FetchProducts();*/
        void Initialize();
        void Fetch();
        bool IsPaymentsAvailable();
        bool IsPremiumBought();
        bool IsSupportBought();
        void PurchasePremium();
        void PurchaseSupport();
        void StartPurchase(string purchaseIdOrTag);
        ProductData GetProductDataOrNull(string tag);
        void CheckIAPRewardsGained();
    }
}
