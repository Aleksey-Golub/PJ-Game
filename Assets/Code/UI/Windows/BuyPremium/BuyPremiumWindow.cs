using Code.Services;
using Code.UI.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class BuyPremiumWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _premiumBtn;
        [SerializeField] private TextMeshProUGUI _premiumText;

        private IIAPService _iapService;

        internal void Construct(IUIMediator uiMediator, IAudioService audio, IIAPService iapService)
        {
            base.Construct(uiMediator, audio);

            _iapService = iapService;
        }

        internal void Open()
        {
            gameObject.SetActive(true);

            RefreshUI();
        }

        public override void Close()
        {
            CloseSelf();
        }

        protected override void SubscribeUpdates()
        {
            _iapService.Purchased += OnSomePurchased;
            _premiumBtn.onClick.AddListener(BuyPremium);

            LService.LanguageChanged += RefreshUI;
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            _iapService.Purchased -= OnSomePurchased;
            _premiumBtn.onClick.RemoveListener(BuyPremium);

            LService.LanguageChanged -= RefreshUI;
        }

        private void CloseSelf() => gameObject.SetActive(false);

        private void RefreshPremium()
        {
            bool premiumBought = _iapService.IsPremiumBought();
            _premiumBtn.interactable = !premiumBought;

            var premiumData = _iapService.GetProductDataOrNull(Constants.PREMIUM_TAG);
            _premiumText.text = $"{premiumData?.price} {premiumData?.currencySymbol}";
        }

        private void RefreshUI()
        {
            _header.text = LService.Localize("k_Premium_header");
#if GAME_PUSH
            if (GamePush.GP_Platform.Type() is GamePush.Platform.YANDEX)
            {
                _description.text = LService.Localize("k_Premium_description_YG");
            }
            else
            {
                _description.text = LService.Localize("k_Premium_description");
            }
#else
            _description.text = LService.Localize("k_Premium_description");
#endif

            RefreshPremium();
        }

        private void OnSomePurchased(bool isPurchaseSuccessed)
        {
            RefreshPremium();
        }

        private void BuyPremium()
        {
            _iapService.PurchasePremium();
        }
    }
}