using Code.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class BuySupportWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _supportBtn;
        [SerializeField] private TextMeshProUGUI _supportText;

        private IIAPService _iapService;

        internal void Construct(IAudioService audio, IIAPService iapService)
        {
            base.Construct(audio);

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
            _supportBtn.onClick.AddListener(BuySupport);

            LService.LanguageChanged += RefreshUI;
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            _iapService.Purchased -= OnSomePurchased;
            _supportBtn.onClick.RemoveListener(BuySupport);

            LService.LanguageChanged -= RefreshUI;
        }

        protected override void OnCloseButtonClicked()
        {
            base.OnCloseButtonClicked();
            CloseSelf();
        }

        private void CloseSelf() => gameObject.SetActive(false);

        private void RefreshSupport()
        {
            bool supportBought = _iapService.IsSupportBought();
            _supportBtn.interactable = !supportBought;

            var premiumData = _iapService.GetProductDataOrNull(Constants.SUPPORT_TAG);
            _supportText.text = $"{premiumData?.price} {premiumData?.currencySymbol}";
        }

        private void RefreshUI()
        {
            _header.text = LService.Localize("k_Support_header");
            _description.text = LService.Localize("k_Support_description");

            RefreshSupport();
        }

        private void OnSomePurchased(bool isPurchaseSuccessed)
        {
            RefreshSupport();
        }

        private void BuySupport()
        {
            _iapService.PurchaseSupport();
        }
    }
}