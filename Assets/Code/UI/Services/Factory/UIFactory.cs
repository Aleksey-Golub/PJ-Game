using Code.Infrastructure;
using Code.Services;
using System;
using UnityEngine;

namespace Code.UI.Services
{
    public class UIFactory : IUIFactory
    {
        private const string UI_ROOT_PATH = "UI/UI Root";
        private const string UPGRADEBOARDVIEW_PATH = "UI/Windows/Upgrade/Upgrade Board View";
        private const string SELLBOARDVIEW_PATH = "UI/Windows/Sell/Sell Board View";
        private const string UI_POPUP_PATH = "UI/UI Popups/UI Popup";

        private readonly IAssetProvider _assets;
        private readonly IConfigsService _configs;

        private Transform _uiRoot;
        private readonly IPersistentProgressService _progressService;
        private readonly IAudioService _audio;
        private readonly ISaveLoadAppSettingsService _saveLoadAppSettingsService;
        private readonly IAdsService _adsService;
        private readonly IUpdater _updater;

        public UIFactory(
            IAssetProvider assets,
            IConfigsService configs,
            IPersistentProgressService progressService,
            IAudioService audio,
            ISaveLoadAppSettingsService saveLoadAppSettingsService,
            IAdsService adsService,
            IUpdater updater
            )
        {
            _assets = assets;
            _configs = configs;
            _progressService = progressService;
            _audio = audio;
            _saveLoadAppSettingsService = saveLoadAppSettingsService;
            _adsService = adsService;
            _updater = updater;
        }

        public void CreateUIRoot()
        {
            _uiRoot = _assets.Instantiate(UI_ROOT_PATH).transform;
        }

        public WindowBase CreateWindow(WindowId windowId)
        {
            WindowMatcher config = _configs.GetMatcherFor(windowId);
            WindowBase window = UnityEngine.Object.Instantiate(config.Template, _uiRoot);
            ConstructWindow(window, windowId);

            return window;
        }

        public UIPopup CreatePopup(UIPopupId uiPopupId)
        {
            UIPopup popup = _assets.Instantiate(UI_POPUP_PATH, _uiRoot).GetComponent<UIPopup>();
            ConstructUiPopup(popup, uiPopupId);

            return popup;
        }

        public UpgradeBoardView CreateUpgradeBoardView()
        {
            UpgradeBoardView upgradeBoardView = _assets.Instantiate(UPGRADEBOARDVIEW_PATH, _uiRoot).GetComponent<UpgradeBoardView>();
            upgradeBoardView.Coustruct(_configs, _progressService, _audio, _adsService, _updater);

            return upgradeBoardView;
        }

        public SellBoardView CreateSellBoardView()
        {
            SellBoardView sellBoardView = _assets.Instantiate(SELLBOARDVIEW_PATH, _uiRoot).GetComponent<SellBoardView>();
            sellBoardView.Coustruct(_configs, _audio);

            return sellBoardView;
        }

        private void ConstructWindow(WindowBase window, WindowId windowId)
        {
            switch (windowId)
            {
                case WindowId.Settings:
                    ((SettingsWindow)window).Construct(_audio, _saveLoadAppSettingsService);
                    break;
                //case WindowId.Sell:
                //case WindowId.Upgrade:
                case WindowId.None:
                default:
                    throw new NotImplementedException($"Not implemented for {windowId}");
            }
        }
        
        private void ConstructUiPopup(UIPopup popup, UIPopupId popupId)
        {
            popup.gameObject.name = $"{popupId.ToString()} popup";

            switch (popupId)
            {
                case UIPopupId.AdWaiting:
                    popup.Construct("k_waitingForAd");
                    break;
                case UIPopupId.None:
                default:
                    throw new NotImplementedException($"Not implemented for {popupId}");
            }
        }
    }
}