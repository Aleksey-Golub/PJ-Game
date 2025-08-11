using System;
using System.Collections.Generic;

namespace Code.Services
{
    public class IAPService : IIAPService
    {
        private const string PREMIUM_KEY = "Premium";
        private const string PREMIUM_ID = "PREMIUM";
        private bool? _isPremiumBought;

        public event Action<bool> Purchased;

        /*//
        //Подписка на события
        private void OnEnable()
        {
            GamePush.GP_Payments.OnFetchProducts += OnFetchProducts;
            GamePush.GP_Payments.OnFetchProductsError += OnFetchProductsError;
        }
        //Отписка от событий
        private void OnDisable()
        {
            GamePush.GP_Payments.OnFetchProducts -= OnFetchProducts;
            GamePush.GP_Payments.OnFetchProductsError -= OnFetchProductsError;
        }

        //Можно получить список товаров через метод 
        public void FetchProducts() => GamePush.GP_Payments.Fetch();

        // Успешно получен
        private void OnFetchProducts(List<GamePush.FetchProducts> products)
        {
            Logger.Log($"@@@ FETCH PRODUCTS: count={products.Count}");

            for (int i = 0; i < products.Count; i++)
            {
                Logger.Log("PRODUCT: ID: " + products[i].id);
                Logger.Log("PRODUCT: TAG: " + products[i].tag);
                Logger.Log("PRODUCT: NAME: " + products[i].name);
                Logger.Log("PRODUCT: DESCRIPTION: " + products[i].description);
                Logger.Log("PRODUCT: ICON: " + products[i].icon);
                Logger.Log("PRODUCT: ICON SMALL: " + products[i].iconSmall);
                Logger.Log("PRODUCT: PRICE: " + products[i].price);
                Logger.Log("PRODUCT: CURRENCY: " + products[i].currency);
                Logger.Log("PRODUCT: CURRENCY SYMBOL: " + products[i].currencySymbol);
                Logger.Log("PRODUCT: IS SUBSCRIPTION: " + products[i].isSubscription);
                Logger.Log("PRODUCT: PERIOD: " + products[i].period);
                Logger.Log("PRODUCT: TRIAL PERIOD: " + products[i].trialPeriod);
            }
        }
        // Ошибки при получении
        private void OnFetchProductsError() => Logger.LogError("FETCH PRODUCTS: ERROR");
        //*/

        public void Initialize()
        {
            /*OnEnable();*/
        }

        public bool IsPremiumBought()
        {
            if (!_isPremiumBought.HasValue)
                _isPremiumBought = GamePush.GP_Player.GetBool(PREMIUM_KEY);

            return _isPremiumBought.Value;
        }

        public void PurchasePremium()
        {
            if (IsPremiumBought())
            {
                Logger.LogWarning($"[IAPService] PREMIUM is already bought");
                return;
            }

            StartPurchase(PREMIUM_ID);
        }

        public void StartPurchase(string purchaseId)
        {
            GamePush.GP_Payments.Purchase(purchaseId, OnPurchaseSuccess, OnPurchaseError);
        }

        private void OnPurchaseSuccess(string productIdOrTag)
        {
            if (productIdOrTag == PREMIUM_ID)
            {
                GamePush.GP_Player.Set(PREMIUM_KEY, true);
                _isPremiumBought = true;

                GamePush.GP_Player.Sync();
            }

            Logger.LogWarning($"[IAPService] OnPurchaseSuccess={productIdOrTag}");
            Purchased?.Invoke(true);
        }

        private void OnPurchaseError()
        {
            Logger.LogWarning("[IAPService] OnPurchaseError() PURCHASE: ERROR");
            Purchased?.Invoke(false);
        }
    }
}
