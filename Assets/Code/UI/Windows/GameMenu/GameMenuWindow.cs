using Code.Services;
using Code.UI.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    internal class GameMenuWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _header;

        [SerializeField] private Button _premiumBtn;
        [SerializeField] private TextMeshProUGUI _premiumText;
        
        [SerializeField] private Button _supportBtn;
        [SerializeField] private TextMeshProUGUI _supportText;
        
        [SerializeField] private Button _settingsBtn;
        [SerializeField] private Button _skinsBtn;

        private IIAPService _iapService;
        private IAdsService _adsService;
        private IUIMediator _uiMediator;

        internal void Construct(IAudioService audio, IIAPService iapService, IAdsService adsService, IUIMediator uiMediator)
        {
            base.Construct(uiMediator, audio);

            _iapService = iapService;
            _adsService = adsService;
            _uiMediator = uiMediator;
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
            _supportBtn.onClick.AddListener(BuySupport);

            LService.LanguageChanged += RefreshUI;
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            _iapService.Purchased -= OnSomePurchased;
            _premiumBtn.onClick.RemoveListener(BuyPremium);
            _supportBtn.onClick.RemoveListener(BuySupport);

            LService.LanguageChanged -= RefreshUI;
        }

        private void CloseSelf() => gameObject.SetActive(false);

        private void RefreshPremium()
        {
            bool premiumBought = _iapService.IsPremiumBought();
            bool isPaymentsAvailable = _iapService.IsPaymentsAvailable();
            bool isFullscreenAdsAvailable = _adsService.IsFullscreenAvailable();
            //_noAdsText.text = noAdsBought ? LService. : LService.;
            _premiumBtn.interactable = !premiumBought && isFullscreenAdsAvailable;
            _premiumBtn.gameObject.SetActive(isPaymentsAvailable && isFullscreenAdsAvailable);
        }
        
        private void RefreshSupport()
        {
            bool supportBought = _iapService.IsSupportBought();
            bool isPaymentsAvailable = _iapService.IsPaymentsAvailable();
            //_noAdsText.text = noAdsBought ? LService. : LService.;
            _supportBtn.interactable = !supportBought;
            _supportBtn.gameObject.SetActive(isPaymentsAvailable);

            bool isFullscreenAdsAvailable = _adsService.IsFullscreenAvailable();
            if (!isFullscreenAdsAvailable)
            {
                var position = _supportBtn.gameObject.transform.localPosition;
                position.x = 0;
                _supportBtn.gameObject.transform.localPosition = position;
            }
        }

        private void RefreshSkinsButton()
        {
            bool isPaymentsAvailable = _iapService.IsPaymentsAvailable();
            _skinsBtn.gameObject.SetActive(isPaymentsAvailable);
        }

        private void RefreshUI()
        {
            _header.text = LService.Localize("k_GameMenu_header");

            RefreshPremium();
            RefreshSupport();
            RefreshSkinsButton();
        }

        private void OnSomePurchased(bool isPurchaseSuccessed)
        {
            RefreshPremium();
            RefreshSupport();
        }

        private void BuyPremium() => _uiMediator.Open(WindowId.BuyPremium);
        private void BuySupport() => _uiMediator.Open(WindowId.BuySupport);
    }
}