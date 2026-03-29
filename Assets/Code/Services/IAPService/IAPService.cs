using System;

namespace Code.Services
{
    public class IAPService : IIAPService
    {
        private const string PREMIUM_KEY = "Premium";
        private const string PREMIUM_ID = "PREMIUM";
        private const string SUPPORT_KEY = "Support";
        private const string SUPPORT_ID = "SUPPORT";
        private bool? _isPremiumBought;
        private bool? _isSupportBought;

        private IPersistentProgressService _progress;
        private ISaveLoadService _saveService;

        public event Action<bool> Purchased;

        public void PostConstruct(IPersistentProgressService progress, ISaveLoadService saveLoad)
        {
            _progress = progress;
            _saveService = saveLoad;
        }

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
        public bool IsPaymentsAvailable()
        {
            //return true; //
            return GamePush.GP_Payments.IsPaymentsAvailable();
        }

        public bool IsPremiumBought()
        {
            if (!_isPremiumBought.HasValue)
                _isPremiumBought = GamePush.GP_Player.GetBool(PREMIUM_KEY);

            return _isPremiumBought.Value;
        }
        
        public bool IsSupportBought()
        {
            if (!_isSupportBought.HasValue)
                _isSupportBought = GamePush.GP_Player.GetBool(SUPPORT_KEY);

            return _isSupportBought.Value;
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
        
        public void PurchaseSupport()
        {
            if (IsSupportBought())
            {
                Logger.LogWarning($"[IAPService] SUPPORT is already bought");
                return;
            }

            StartPurchase(SUPPORT_ID);
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

                //GamePush.GP_Player.Sync(); // Metrika syncs too
                Metrika.EventReached(Metrika.Event.Ads_Premium_Bought);
            }
            
            if (productIdOrTag == SUPPORT_ID)
            {
                GamePush.GP_Player.Set(SUPPORT_KEY, true);
                _isSupportBought = true;
                _progress.Progress.PlayerProgress.SkinsData.AddAvailableSkin(SkinId.SupportSkin);
                _saveService.SaveProgress();

                //GamePush.GP_Player.Sync(); // Metrika syncs too
                Metrika.EventReached(Metrika.Event.Ads_Support_Bought);
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
