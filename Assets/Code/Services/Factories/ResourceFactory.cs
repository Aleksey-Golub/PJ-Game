using System.Collections.Generic;
using UnityEngine;
using Code.Infrastructure;

namespace Code.Services
{
    internal class ResourceFactory : IResourceFactory
    {
        private readonly Pool<Resource> _pool;
        private readonly List<Resource> _droppedResources;

        public IReadOnlyList<Resource> DroppedResources => _droppedResources;

        public ResourceFactory(IAudioService audio, IAssetProvider assets)
        {
            Transform container = CreateContainer();
            Resource resourcePrefab = assets.Load<Resource>(AssetPath.RESOURCE_PREFAB_PATH);
            int poolSize = 10;

            _pool = new Pool<Resource>(resourcePrefab, container, poolSize, this, audio);
            _droppedResources = new List<Resource>();
        }

        public Resource Get(Vector3 position, Quaternion rotation)
        {
            var res = _pool.Get(position, rotation);

            _droppedResources.Add(res);

            return res;
        }

        public void Recycle(IPoolable resource)
        {
            _droppedResources.Remove(resource as Resource);

            _pool.Recycle(resource as Resource);
        }

        private Transform CreateContainer()
        {
            var go = new GameObject("Resource Factory Container");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.transform;
        }
    }
}