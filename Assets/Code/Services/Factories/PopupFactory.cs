using Code.Infrastructure;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Services
{
    internal class PopupFactory : IPopupFactory
    {
        private const string CONTAINER_NAME = "Popup Factory Container";

        private Pool<Popup> _pool;
        private Transform _container;
        private readonly List<Popup> _inUseItems;
        private readonly IAssetProvider _assets;

        public PopupFactory(IAssetProvider assets)
        {
            _assets = assets;

            _inUseItems = new List<Popup>();
        }

        public void Load()
        {
            _container = FactoryHelper.CreateDontDestroyOnLoadGameObject(CONTAINER_NAME).transform;
            Popup prefab = _assets.Load<Popup>(AssetPath.POPUP_PREFAB_PATH);
            int poolSize = 10;

            _pool = new Pool<Popup>(prefab, _container, poolSize);
        }

        public void Cleanup()
        {
            _inUseItems.ForEach(i => _pool.Recycle(i));
            _inUseItems.Clear();
            Object.Destroy(_container.gameObject);

            _pool = null;
        }

        public Popup Get(Vector3 position, Quaternion rotation)
        {
            Popup popup = _pool.Get(position, rotation);

            if (!popup.IsConstructed)
                popup.Construct(this);

            _inUseItems.Add(popup);
            return popup;
        }

        public void Recycle(IPoolable popup)
        {
            Popup item = popup as Popup;
            _inUseItems.Remove(item);

            _pool.Recycle(item);
        }
    }
}