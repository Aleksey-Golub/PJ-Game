using System;

namespace Code.Services
{
    public interface IIAPService : IService
    {
        event Action<bool> Purchased;

        /*void FetchProducts();*/
        void Initialize();
        bool IsPremiumBought();
        void PurchasePremium();
        void StartPurchase(string v);
    }
}
