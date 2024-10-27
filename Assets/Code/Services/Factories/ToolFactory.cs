using UnityEngine;
using Code.Infrastructure;

namespace Code.Services
{
    internal class ToolFactory : IToolFactory
    {
        private readonly Pool<Tool> _pool;

        public ToolFactory(IAudioService audio, IAssetProvider assets)
        {
            Transform container = CreateContainer();
            Tool prefab = assets.Load<Tool>(AssetPath.TOOL_PREFAB_PATH);
            int poolSize = 10;

            _pool = new Pool<Tool>(prefab, container, poolSize, this, audio);
        }

        public Tool Get(Vector3 position, Quaternion rotation)
        {
            var tool = _pool.Get(position, rotation);

            return tool;
        }

        public void Recycle(IPoolable tool)
        {
            _pool.Recycle(tool as Tool);
        }

        private Transform CreateContainer()
        {
            var go = new GameObject("Tool Factory Container");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.transform;
        }
    }
}