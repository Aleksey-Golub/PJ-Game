using UnityEngine;
using Code.Infrastructure;

namespace Code.Services
{
    internal class TransitionalResourceFactory : ITransitionalResourceFactory
    {
        private readonly Pool<TransitionalResource> _pool;

        public TransitionalResourceFactory(IAudioService audio, IAssetProvider assets)
        {
            Transform container = CreateContainer();
            TransitionalResource prefab = assets.Load<TransitionalResource>(AssetPath.TRANSITIONALRESOURCE_PREFAB_PATH);
            int poolSize = 10;

            _pool = new Pool<TransitionalResource>(prefab, container, poolSize, this, audio);
        }

        public TransitionalResource Get(Vector3 position, Quaternion rotation)
        {
            var popup = _pool.Get(position, rotation);

            return popup;
        }

        public void Recycle(IPoolable popup)
        {
            _pool.Recycle(popup as TransitionalResource);
        }

        private Transform CreateContainer()
        {
            var go = new GameObject("Transitional Resource Factory Container");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.transform;
        }
    }
}