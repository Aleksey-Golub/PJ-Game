using Code.Data;
using Code.Services;
using Code.UI.Services;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    internal class CharacterSkinsWindow : WindowBase
    {
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private TextMeshProUGUI _description;

        [SerializeField] private SkinCardView _prefab;
        [SerializeField] private Transform _content;
        [SerializeField] private SkinButtonColors _skinButtonColors;

        private IConfigsService _configService;
        private IPersistentProgressService _progressService;
        private IUIMediator _uiMediator;

        private Dictionary<SkinId, SkinCardView> _views;

        private SkinsData SkinsData => _progressService.Progress.PlayerProgress.SkinsData;

        internal void Construct(IAudioService audio, IConfigsService configService, IPersistentProgressService progressService, IUIMediator uiMediator)
        {
            base.Construct(audio);

            _configService = configService;
            _progressService = progressService;
            _uiMediator = uiMediator;

            _views = new();
            LService.LanguageChanged += RefreshUI;

            FillViews();
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
            LService.LanguageChanged += RefreshUI;
            SkinsData.SelectedSkinChanged += SelectedSkinChanged;
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            LService.LanguageChanged -= RefreshUI;
            SkinsData.SelectedSkinChanged -= SelectedSkinChanged;
        }

        protected override void OnCloseButtonClicked()
        {
            base.OnCloseButtonClicked();
            CloseSelf();
        }

        private void CloseSelf() => gameObject.SetActive(false);

        private void RefreshUI()
        {
            _header.text = LService.Localize("k_Skins");
            _description.text = LService.Localize("k_Skins_description");
        }

        private void FillViews()
        {
            foreach (SkinId skinId in Enum.GetValues(typeof(SkinId)))
            {
                if (skinId is SkinId.None)
                    continue;

                var skinView = Instantiate(_prefab, _content);
                skinView.Construct(Audio);

                skinView.ButtonClicked += OnSkinSelectButtonClicked;
                _views.Add(skinId, skinView);
            }

            SetViewsData();
        }

        private void SetViewsData()
        {
            foreach (var viewpair in _views)
            {
                SkinId skinId = viewpair.Key;
                SkinCardView skinView = viewpair.Value;

                var config = _configService.GetConfigFor(skinId);
                Sprite skinSprite = config.Preview;
                bool isSelected = SkinsData.SelectedSkinId == skinId;
                bool obtained = SkinsData.AvailableSkins.Contains(skinId);
                bool isUnique = config.IsSpecialUniqueSkin;
                var props = GetSkinCardButtonProps(isSelected, obtained, isUnique);

                skinView.SetData(skinId, skinSprite, props);
            }

            (string locKey, Color color, bool interactable) GetSkinCardButtonProps(bool isSelected, bool obtained, bool isUnique)
            {
                if (isSelected)
                    return ("k_Skins_selected", _skinButtonColors.Selected, true);

                if (obtained)
                    return ("k_Skins_select", _skinButtonColors.UnselectedObtained, true);

                if (!isUnique)
                    return ("k_Skins_unavailable", _skinButtonColors.UnselectedNotUnique, false);

                return ("k_Skins_obtain", _skinButtonColors.UnselectedUnique, true);
            }
        }

        private void OnSkinSelectButtonClicked(SkinId skinId)
        {
            var config = _configService.GetConfigFor(skinId);
            bool isSelected = SkinsData.SelectedSkinId == skinId;
            bool obtained = SkinsData.AvailableSkins.Contains(skinId);
            bool isUnique = config.IsSpecialUniqueSkin;

            if (isSelected)
                return;

            if (obtained)
            {
                SkinsData.SelecteSkin(skinId);
                return;
            }

            if (!isUnique)
                return;

            HandleUniqueSkinButtonCkicked(skinId);
            return;
        }

        private void HandleUniqueSkinButtonCkicked(SkinId skinId)
        {
            switch (skinId)
            {
                case SkinId.SupportSkin:
                    _uiMediator.Open(WindowId.BuySupport);
                    break;
                case SkinId.BaseSkin:
                    break;
                case SkinId.None:
                default:
                    Logger.LogWarning($"[CharacterSkinsWindow.HandleUniqueSkinButtonCkicked] there is no hanler for skinId='{skinId}'");
                    break;
            }
        }

        private void SelectedSkinChanged(SkinId skinId) => SetViewsData();

        [Serializable]
        public class SkinButtonColors
        {
            public Color Selected = new(89, 96, 100);
            public Color UnselectedObtained = new(55, 88, 96);
            public Color UnselectedNotUnique = new(55, 88, 96);
            public Color UnselectedUnique = new(183, 89, 96);
        }
    }
}