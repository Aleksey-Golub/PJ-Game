using System.Collections;
using UnityEngine;

internal class Resource : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ResourceView _view;

    private ResourceFactory _factory;
    private ResourceConfig _config;
    private Coroutine _moveCoroutine;

    [Header("Settings")]
    [SerializeField] private float _dropRadius = 1.3f;
    [SerializeField] private float _moveAfterDropTime = 0.6f;

    internal int Count { get; private set; }
    internal ResourceType Type => _config.Type;

    internal void Construct(ResourceFactory factory)
    {
        _factory = factory;
    }

    internal void Init(ResourceConfig config)
    {
        _config = config;
        Count = 1;

        _view.Init(_config.Sprite);
        _collider.enabled = false;
    }

    internal void MoveAfterDrop()
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveAfterDropCor());
    }

    private IEnumerator MoveAfterDropCor()
    {
        _view.ShowStartDrop();
        Vector3 finalPosition = Random.insideUnitCircle * _dropRadius + new Vector2(transform.position.x, transform.position.y);
        
        finalPosition.z = transform.position.z;
        Vector3 startPosition = transform.position;
        float timer = 0;

        while (timer < _moveAfterDropTime)
        {
            float t = timer / _moveAfterDropTime;
            transform.position = Vector3.Slerp(startPosition, finalPosition, t);
            _view.ShowDropping(t);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = finalPosition;
        _collider.enabled = true;
        _view.ShowEndDrop();
    }

    internal void Collect()
    {
        AudioSource.PlayClipAtPoint(_config.PickupAudio, transform.position);

        _factory.Recycle(this);
    }
}
