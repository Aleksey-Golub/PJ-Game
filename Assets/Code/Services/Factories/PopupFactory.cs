using UnityEngine;

internal class PopupFactory : MonoSingleton<PopupFactory>
{
    [SerializeField] private Popup _popupPrefab;
    [SerializeField, Min(1)] private int _poolSize = 1;

    private Pool<Popup> _pool;

    protected override void Awake()
    {
        base.Awake();

        _pool = new Pool<Popup>(_popupPrefab, transform, _poolSize);
    }

    internal Popup Get(Vector3 position, Quaternion rotation)
    {
        var popup = _pool.Get(position, rotation);
        popup.Construct(this);

        return popup;
    }

    internal void Recycle(Popup popup)
    {
        _pool.Recycle(popup);
    }
}
