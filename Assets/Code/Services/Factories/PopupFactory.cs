using Code.Infrastructure;
using UnityEngine;

namespace Code.Services
{
    internal class PopupFactory : IPopupFactory
    {
        private readonly Pool<Popup> _pool;

        public PopupFactory(IAssetProvider assets)
        {
            Transform container = CreateContainer();
            Popup prefab = assets.Load<Popup>(AssetPath.POPUP_PREFAB_PATH);
            int poolSize = 10;

            _pool = new Pool<Popup>(prefab, container, poolSize);
        }

        public Popup Get(Vector3 position, Quaternion rotation)
        {
            var popup = _pool.Get(position, rotation);

            if (!popup.IsConstructed)
                popup.Construct(this);

            return popup;
        }

        public void Recycle(IPoolable popup)
        {
            _pool.Recycle(popup as Popup);
        }

        private Transform CreateContainer()
        {
            var go = new GameObject("Popup Factory Container");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.transform;
        }
    }
}