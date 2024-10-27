using Code.Infrastructure;
using Code.Services;
using System;
using UnityEngine;

namespace Code.UI.Services
{
    public class UIFactory : IUIFactory
    {
        private const string UI_ROOT_PATH = "UI/UI Root";
        private const string UPGRADEBOARDVIEW_PATH = "UI/Windows/Upgrade Board View";
        private const string SELLBOARDVIEW_PATH = "UI/Windows/Sell Board View";

        private readonly IAssetProvider _assets;
        private readonly IConfigsService _configs;

        private Transform _uiRoot;
        private readonly IPersistentProgressService _progressService;
        private readonly IAudioService _audio;

        public UIFactory(IAssetProvider assets, IConfigsService configs, IPersistentProgressService progressService, IAudioService audio)
        {
            _assets = assets;
            _configs = configs;
            _progressService = progressService;
            _audio = audio;
        }

        public void CreateUIRoot()
        {
            _uiRoot = _assets.Instantiate(UI_ROOT_PATH).transform;
        }

        public WindowBase CreateWindow(WindowId windowId)
        {
            WindowConfig config = _configs.GetConfigFor(windowId);
            WindowBase window = UnityEngine.Object.Instantiate(config.Template, _uiRoot);
            ConstructWindow(window, windowId);

            return window;
        }

        public UpgradeBoardView CreateUpgradeBoardView()
        {
            UpgradeBoardView upgradeBoardView = _assets.Instantiate(UPGRADEBOARDVIEW_PATH, _uiRoot).GetComponent<UpgradeBoardView>();
            upgradeBoardView.Coustruct(_configs, _progressService, _audio);

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
                    ((SettingsWindow)window).Construct(_audio);
                    break;
                //case WindowId.Sell:
                //case WindowId.Upgrade:
                case WindowId.None:
                default:
                    throw new NotImplementedException($"Not implemented for {windowId}");
            }
        }
    }
}