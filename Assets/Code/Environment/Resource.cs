using UnityEngine;

internal class Resource : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ResourceView _view;

    private ResourceConfig _config;

    internal void Init(ResourceConfig config)
    {
        _config = config;

        _view.Init(_config.Sprite);
    }

    internal void MoveAfterDrop()
    {
        transform.position = Random.insideUnitCircle + new Vector2(transform.position.x, transform.position.y);
    }
}
