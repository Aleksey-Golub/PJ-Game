using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.UI
{
    internal class PlayerInventoryView : MonoBehaviour, IInventoryView
    {
        [SerializeField] private InventoryResourceView _prefab;

        private ResourceConfigService _resourceConfigService;
        private Dictionary<ResourceType, InventoryResourceView> _views;

        internal void Coustruct(ResourceConfigService configService)
        {
            _views = new();

            _resourceConfigService = configService;
        }

        public void Init(IReadOnlyDictionary<ResourceType, int> storage)
        {
            foreach (var pair in storage)
            {
                InventoryResourceView resView = CreateView(pair.Key, pair.Value);

                _views.Add(pair.Key, resView);
            }
        }

        public void UpdateFor(ResourceType resourceType, int newCount)
        {
            if (!_views.TryGetValue(resourceType, out InventoryResourceView resView))
            {
                resView = CreateView(resourceType, newCount);
            }

            resView.Set(newCount);
        }

        private InventoryResourceView CreateView(ResourceType type, int initialValue)
        {
            var resView = Instantiate(_prefab, transform);
            var config = _resourceConfigService.GetConfigFor(type);
            var sprite = config.Sprite;

            resView.Init(sprite, initialValue);

            return resView;
        }
    }
}