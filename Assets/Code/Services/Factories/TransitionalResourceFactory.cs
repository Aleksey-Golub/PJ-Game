using UnityEngine;
using Code.Infrastructure;
using System.Collections.Generic;

namespace Code.Services
{
    internal class TransitionalResourceFactory : ITransitionalResourceFactory
    {
        private Pool<TransitionalResource> _pool;
        private Transform _container;
        private readonly List<TransitionalResource> _inUseItems;
        private readonly IAudioService _audio;
        private readonly IAssetProvider _assets;

        public TransitionalResourceFactory(IAudioService audio, IAssetProvider assets)
        {
            _audio = audio;
            _assets = assets;

            _inUseItems = new List<TransitionalResource>();
        }

        public void Load()
        {
            _container = CreateContainer();
            TransitionalResource prefab = _assets.Load<TransitionalResource>(AssetPath.TRANSITIONALRESOURCE_PREFAB_PATH);
            int poolSize = 10;

            _pool = new Pool<TransitionalResource>(prefab, _container, poolSize);
        }

        public void Cleanup()
        {
            _inUseItems.ForEach(i => _pool.Recycle(i));
            _inUseItems.Clear();
            Object.Destroy(_container.gameObject);

            _pool = null;
        }

        public TransitionalResource Get(Vector3 position, Quaternion rotation)
        {
            TransitionalResource tResource = _pool.Get(position, rotation);

            if (!tResource.IsConstructed)
                tResource.Construct(this, _audio);

            _inUseItems.Add(tResource);
            return tResource;
        }

        public void Recycle(IPoolable tResource)
        {
            TransitionalResource item = tResource as TransitionalResource;
            _inUseItems.Remove(item);

            _pool.Recycle(item);
        }

        private Transform CreateContainer()
        {
            var go = new GameObject("Transitional Resource Factory Container");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.transform;
        }
    }
}