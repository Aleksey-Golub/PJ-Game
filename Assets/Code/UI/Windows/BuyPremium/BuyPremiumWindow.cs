using Code.Services;
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

        protected override void OnCloseButtonClicked()
        {
            base.OnCloseButtonClicked();
            CloseSelf();
        }

        private void CloseSelf() => gameObject.SetActive(false);

        private void RefreshPremium()
        {
            bool premiumBought = _iapService.IsPremiumBought();
            //_noAdsText.text = noAdsBought ? LService. : LService.;
            _premiumBtn.interactable = !premiumBought;
        }

        private void RefreshUI()
        {
            _header.text = LService.Localize("k_Premium_header");
            _description.text = LService.Localize("k_Premium_description");

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