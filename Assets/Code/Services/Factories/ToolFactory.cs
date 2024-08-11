using UnityEngine;

internal class ToolFactory : MonoSingleton<ToolFactory>
{
    [SerializeField] private Tool _toolPrefab;
    [SerializeField, Min(1)] private int _poolSize = 1;

    private Pool<Tool> _pool;

    protected override void Awake()
    {
        base.Awake();

        _pool = new Pool<Tool>(_toolPrefab, transform, _poolSize);
    }

    internal Tool Get(Vector3 position, Quaternion rotation)
    {
        var tool = _pool.Get(position, rotation);
        tool.Construct(this);

        return tool;
    }

    internal void Recycle(Tool resource)
    {
        _pool.Recycle(resource);
    }
}
