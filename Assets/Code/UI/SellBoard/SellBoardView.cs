using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.UI
{
    internal class SellBoardView : MonoBehaviour
    {
        [SerializeField] private SellItemView _prefab;
        [SerializeField] private Transform _content;
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private Button _closeButton;
        [SerializeField] private AudioClip _closeButtonClickedClip;

        private ConfigsService _resourceConfigService;
        private Dictionary<ResourceType, SellItemView> _views;
        private Action<ResourceType> _sellResourceCalback;

        internal Action<ResourceType> SellButtonClicked;
        private AudioService _audio;

        internal void Coustruct(ConfigsService configService, AudioService audio)
        {
            _views = new();
            _resourceConfigService = configService;
            _audio = audio;

            FillViews();

            _closeButton.onClick.AddListener(CloseFromUI);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(CloseFromUI);

            foreach (var view in _views.Values)
            {
                view.SellButtonClicked -= OnSellButtonClicked;
            }
        }

        internal void Open(IReadOnlyDictionary<ResourceType, int> storage, Action<ResourceType> sellResourceCalback)
        {
            _sellResourceCalback = sellResourceCalback;
            gameObject.SetActive(true);
            
            Refresh(storage);
        }

        internal void Refresh(IReadOnlyDictionary<ResourceType, int> storage)
        {
            foreach (var pair in storage)
            {
                ResourceType type = pair.Key;

                if (_views.TryGetValue(type, out SellItemView resView))
                {
                    int itemsCount = pair.Value;
                    resView.SetData(itemsCount, itemsCount * _resourceConfigService.ResourcesConfigs[type].Cost);
                }
            }
        }

        internal void Close()
        {
            _sellResourceCalback = null;
            gameObject.SetActive(false);
        }

        private void CloseFromUI()
        {
            _audio.PlaySfxAtPosition(_closeButtonClickedClip, Camera.main.transform.position);
            Close();
        }

        private void FillViews()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                if (type is ResourceType.None)
                    continue;

                var config = _resourceConfigService.GetConfigFor(type);
                if (!config.Sellable)
                    continue;
                
                var resView = Instantiate(_prefab, _content);
                resView.Construct(_audio);

                var sprite = config.Sprite;
                resView.Init(sprite, 0, 0, type);
                resView.SellButtonClicked += OnSellButtonClicked;

                _views.Add(type, resView);
            }
        }

        private void OnSellButtonClicked(ResourceType resourceType)
        {
            _sellResourceCalback?.Invoke(resourceType);
        }
    }
}