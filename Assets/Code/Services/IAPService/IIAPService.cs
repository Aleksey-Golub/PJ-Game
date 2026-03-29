using System;

namespace Code.Services
{
    public interface IIAPService : IService
    {
        event Action<bool> Purchased;

        void PostConstruct(IPersistentProgressService progressService, ISaveLoadService saveService);
        /*void FetchProducts();*/
        void Initialize();
        bool IsPaymentsAvailable();
        bool IsPremiumBought();
        bool IsSupportBought();
        void PurchasePremium();
        void PurchaseSupport();
        void StartPurchase(string purchaseId);
    }
}
