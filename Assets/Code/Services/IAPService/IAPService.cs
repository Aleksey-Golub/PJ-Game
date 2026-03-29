using Code.Data;
using GamePush;
using System;
using System.Collections.Generic;
using System.Text;

namespace Code.Services
{
    public class IAPService : IIAPService
    {
        private const string PREMIUM_TAG = Constants.PREMIUM_TAG;
        private const string SUPPORT_TAG = Constants.SUPPORT_TAG;
        private const string PREMIUM_KEY = "Premium";
        private const string SUPPORT_KEY = "Support";
        private bool? _isPremiumBought;
        private bool? _isSupportBought;
        private readonly Dictionary<string, ProductData> _products = new();

        private IPersistentProgressService _progress;
        private ISaveLoadService _saveService;
        private ISaveLoadIAPDataService _saveLoadIAPDataService;

        public PlayerIAPsData PlayerIAPsData { get; set; }

        public event Action<bool> Purchased;

        public void PostConstruct(IPersistentProgressService progress, ISaveLoadService saveLoad, ISaveLoadIAPDataService saveLoadIAPDataService)
        {
            _progress = progress;
            _saveService = saveLoad;
            _saveLoadIAPDataService = saveLoadIAPDataService;
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
            GamePush.GP_Payments.OnFetchProducts += OnFetchProducts;
            GamePush.GP_Payments.OnFetchProductsError += OnFetchProductsError;
            GP_Payments.OnFetchPlayerPurchases += OnFetchPlayerPurchases;
        }

        public void Fetch() => GamePush.GP_Payments.Fetch();

        public bool IsPaymentsAvailable()
        {
            //return true; //
            return GamePush.GP_Payments.IsPaymentsAvailable();
        }

        public bool IsPremiumBought()
        {
            //return false; //

            if (!_isPremiumBought.HasValue)
                _isPremiumBought = GamePush.GP_Player.GetBool(PREMIUM_KEY);

            return _isPremiumBought.Value;
        }

        public bool IsSupportBought()
        {
            //return false; //

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

            StartPurchase(PREMIUM_TAG);
        }

        public void PurchaseSupport()
        {
            if (IsSupportBought())
            {
                Logger.LogWarning($"[IAPService] SUPPORT is already bought");
                return;
            }

            StartPurchase(SUPPORT_TAG);
        }

        public void StartPurchase(string purchaseIdOrTag)
        {
            GamePush.GP_Payments.Purchase(purchaseIdOrTag, OnPurchaseSuccess, OnPurchaseError);
        }

        public ProductData GetProductDataOrNull(string tag)
        {
            return _products.GetValueOrDefault(tag);
        }

        public void CheckIAPRewardsGained()
        {
            Logger.Log($"[IAPService] CheckIAPRewardsGained() called...");

            if (IsSupportBought())
                _progress.Progress.PlayerProgress.SkinsData.TryAddAvailableSkin(SkinId.SupportSkin);
        }

        private void OnPurchaseSuccess(string productIdOrTag) => OnPurchaseSuccess(productIdOrTag, PurchaseSource.RealPurchase, SaveLoadHelper.GetNowTimeToString());

        private void OnPurchaseSuccess(string productIdOrTag, PurchaseSource source, string boughtTime)
        {
            if (productIdOrTag == PREMIUM_TAG)
            {
                GamePush.GP_Player.Set(PREMIUM_KEY, true);
                _isPremiumBought = true;
                GamePush.GP_Player.Sync();

                if (source is PurchaseSource.RealPurchase)
                    Metrika.EventReached(Metrika.Event.Ads_Premium_Bought);
            }

            if (productIdOrTag == SUPPORT_TAG)
            {
                GamePush.GP_Player.Set(SUPPORT_KEY, true);
                _isSupportBought = true;
                _progress.Progress.PlayerProgress.SkinsData.TryAddAvailableSkin(SkinId.SupportSkin);
                _saveService.SaveProgress();
                GamePush.GP_Player.Sync();

                if (source is PurchaseSource.RealPurchase)
                    Metrika.EventReached(Metrika.Event.Ads_Support_Bought);
            }

            PlayerIAPsData.PurchasedIAPsData.AddData(new PurchasedIAPDTO(productIdOrTag, boughtTime, source));
            _saveLoadIAPDataService.SaveIAPData();

            if (source is PurchaseSource.RealPurchase)
                Logger.Log($"[IAPService] OnPurchaseSuccess={productIdOrTag}, source='{source}', boughtTime='{boughtTime}'");
            else
                Logger.Log($"[IAPService] OnPurchase from fetch, idOrTag={productIdOrTag}, source='{source}', boughtTime='{boughtTime}'");
            
            Purchased?.Invoke(true);
        }

        private void OnPurchaseError()
        {
            Logger.LogWarning("[IAPService] OnPurchaseError() PURCHASE: ERROR");
            Purchased?.Invoke(false);
        }

        private void OnFetchProducts(List<GamePush.FetchProducts> products)
        {
            Logger.Log($"[IAPService] FETCH PRODUCTS: count={products.Count}");

            for (int i = 0; i < products.Count; i++)
            {
                _products.Add(products[i].tag, CreateProductData(products[i]));
            }
        }

        private void OnFetchProductsError() => Logger.LogError("[IAPService] FETCH PRODUCTS: ERROR");

        private void OnFetchPlayerPurchases(List<FetchPlayerPurchases> purchases)
        {
            Logger.Log($"[IAPService] FETCH PLAYER PURCHASES: count={purchases.Count}");

            LogPurchasesData(purchases);
            LogPlayerPurchasesData();

            // Dictionary <tag, <createdAt, gift>>
            var serverPurchases = new Dictionary<string, List<(string createdAt, bool isGift)>>();
            for (int i = 0; i < purchases.Count; i++)
            {
                FetchPlayerPurchases purchase = purchases[i];
                if (serverPurchases.TryGetValue(purchase.tag, out var data) == false)
                {
                    data = new List<(string, bool)>();
                    serverPurchases[purchase.tag] = data;
                }

                data.Add((purchase.createdAt, purchase.gift));
            }

            // compare server ad local purchases to re-give purchase
            // validate cheaters
            foreach (var kvp in serverPurchases)
            {
                string tag = kvp.Key;
                List<(string createdAt, bool isGift)> serverDatas = kvp.Value;
                PurchasedIAPData localPurchaseData = PlayerIAPsData.PurchasedIAPsData.GetDataOrNull(tag);

                if (localPurchaseData == null)
                    PlayerIAPsData.PurchasedIAPsData.TryAddEmptyData(tag, out localPurchaseData);

                if (localPurchaseData != null && localPurchaseData.Datas.Count > serverDatas.Count)
                {
                    // looks like player has more purchases then server
                    // looks like player is CHEATER ???
                    LogCheaterWarning(localPurchaseData, serverDatas);

                    continue;
                }

                for (int i = 0; i < serverDatas.Count; i++)
                {
                    (string createdAt, bool isGift) serverData = serverDatas[i];

                    if (localPurchaseData.Datas.Count >= i + 1)
                    {
                        // purchase data exists locally
                    }
                    else
                    {
                        // data exist on server but not exist locally
                        string boughtDate = serverData.createdAt;
                        PurchaseSource source = serverData.isGift ? PurchaseSource.Gift : PurchaseSource.UnhandledPurchase;
            
                        OnPurchaseSuccess(tag, source, boughtDate);
                    }
                }
            }
        }

        private ProductData CreateProductData(FetchProducts fetchProducts)
        {
            return new ProductData()
            {
                id = fetchProducts.id,
                tag = fetchProducts.tag,
                name = fetchProducts.name,
                description = fetchProducts.description,
                icon = fetchProducts.icon,
                iconSmall = fetchProducts.iconSmall,
                price = fetchProducts.price,
                currency = fetchProducts.currency,
                currencySymbol = fetchProducts.currencySymbol,
                isSubscription = fetchProducts.isSubscription,
                period = fetchProducts.period,
                trialPeriod = fetchProducts.trialPeriod,
            };
        }

        private void LogPurchasesData(List<FetchPlayerPurchases> purchases)
        {
            Logger.Log($"[IAPService] LogPurchasesData");

            for (int i = 0; i < purchases.Count; i++)
            {
                Logger.Log("[IAPService] PLAYER PURCHASES: PRODUCT TAG: " + purchases[i].tag);
                Logger.Log("[IAPService] PLAYER PURCHASES: PRODUCT ID: " + purchases[i].productId);
                Logger.Log("[IAPService] PLAYER PURCHASES: PAYLOAD: " + purchases[i].payload);
                Logger.Log("[IAPService] PLAYER PURCHASES: CREATED AT: " + purchases[i].createdAt);
                Logger.Log("[IAPService] PLAYER PURCHASES: EXPIRED AT: " + purchases[i].expiredAt);
                Logger.Log("[IAPService] PLAYER PURCHASES: GIFT: " + purchases[i].gift);
                Logger.Log("[IAPService] PLAYER PURCHASES: SUBSCRIBED: " + purchases[i].subscribed);
            }
        }

        private void LogCheaterWarning(PurchasedIAPData localPurchaseData, List<(string createdAt, bool isGift)> serverDatas)
        {
            Logger.LogWarning($"[IAPService] looks like player is CHEATER: tagOrId='{localPurchaseData.ProductIdOrTag}', localCount:{localPurchaseData.Datas.Count}, serverCount='{serverDatas.Count}'");
        }

        private void LogPlayerPurchasesData()
        {
            Logger.Log($"[IAPService] LogPlayerPurchasesData");

#if GAME_PUSH
            if (PlayerIAPsData is null)
            {
                Logger.LogWarning($"[IAPService] PlayerIAPsData is null. It is OK for GP in Editor");
                return;
            }
#endif

            StringBuilder sb = new();
            sb.AppendLine($"[IAPService] Player local IAPs:");

            foreach (PurchasedIAPData purchaseData in PlayerIAPsData.PurchasedIAPsData.PurchasedIAPs)
            {
                sb.AppendLine($"tagOrId: {purchaseData.ProductIdOrTag}");

                foreach (PurchasedIAPData.Item item in purchaseData.Datas)
                    sb.AppendLine(item.ToString());
            }

            Logger.Log(sb.ToString());
        }
    }
}
