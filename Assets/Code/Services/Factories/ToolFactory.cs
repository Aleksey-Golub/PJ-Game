using UnityEngine;
using Code.Infrastructure;
using System.Collections.Generic;

namespace Code.Services
{
    internal class ToolFactory : IToolFactory
    {
        private Pool<Tool> _pool;
        private Transform _container;
        private readonly List<Tool> _inUseItems;
        private readonly IAudioService _audio;
        private readonly IAssetProvider _assets;
        private readonly IPersistentProgressService _progressService;

        public IReadOnlyList<Tool> DroppedResources => _inUseItems;

        public ToolFactory(IAudioService audio, IAssetProvider assets, IPersistentProgressService progressService)
        {
            _audio = audio;
            _assets = assets;
            _progressService = progressService;

            _inUseItems = new List<Tool>();
        }

        public void Load()
        {
            _container = CreateContainer();
            Tool prefab = _assets.Load<Tool>(AssetPath.TOOL_PREFAB_PATH);
            int poolSize = 10;

            _pool = new Pool<Tool>(prefab, _container, poolSize);
        }

        public void Cleanup()
        {
            _inUseItems.ForEach(i => _pool.Recycle(i));
            _inUseItems.Clear();
            Object.Destroy(_container.gameObject);

            _pool = null;
        }

        public Tool Get(Vector3 position, Quaternion rotation)
        {
            var tool = _pool.Get(position, rotation);

            if (!tool.IsConstructed)
                tool.Construct(this, _audio, _progressService);

            tool.UniqueId.GenerateId();
            
            _inUseItems.Add(tool);
            return tool;
        }

        public void Recycle(IPoolable tool)
        {
            Tool item = tool as Tool;
            _inUseItems.Remove(item);

            _pool.Recycle(item);
        }

        private Transform CreateContainer()
        {
            var go = new GameObject("Tool Factory Container");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.transform;
        }
    }
}