using System.Collections.Generic;
using UnityEngine;
using Code.Infrastructure;

namespace Code.Services
{
    internal class ResourceFactory : IResourceFactory
    {
        private const string CONTAINER_NAME = "Resource Factory Container";

        private Pool<Resource> _pool;
        private Transform _container;
        private readonly List<Resource> _inUseItems;
        private readonly IAudioService _audio;
        private readonly IAssetProvider _assets;
        private readonly IPersistentProgressService _progressService;

        public IReadOnlyList<Resource> DroppedResources => _inUseItems;

        public ResourceFactory(IAudioService audio, IAssetProvider assets, IPersistentProgressService progressService)
        {
            _audio = audio;
            _assets = assets;
            _progressService = progressService;

            _inUseItems = new List<Resource>();
        }

        public void Load()
        {
            _container = FactoryHelper.CreateDontDestroyOnLoadGameObject(CONTAINER_NAME).transform;
            Resource resourcePrefab = _assets.Load<Resource>(AssetPath.RESOURCE_PREFAB_PATH);
            int poolSize = 10;

            _pool = new Pool<Resource>(resourcePrefab, _container, poolSize);
        }

        public void Cleanup()
        {
            _inUseItems.ForEach(i => _pool.Recycle(i));
            _inUseItems.Clear();
            Object.Destroy(_container.gameObject);

            _pool = null;
        }

        public Resource Get(Vector3 position, Quaternion rotation)
        {
            Resource res = _pool.Get(position, rotation);

            if (!res.IsConstructed)
                res.Construct(this, _audio, _progressService);

            res.UniqueId.GenerateId();

            _inUseItems.Add(res);

            return res;
        }

        public void Recycle(IPoolable resource)
        {
            Resource item = resource as Resource;
            _inUseItems.Remove(item);

            _pool.Recycle(item);
        }
    }
}