using System.Collections;
using UnityEngine;

public class Resource : MonoBehaviour, IMergingResource
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ResourceView _view;

    private ResourceFactory _factory;
    private ResourceConfig _config;
    private Coroutine _moveCoroutine;
    private Coroutine _mergeCoroutine;
    private int _count;
    private float _mergeTimer;
    private bool _isMerging;

    [Header("Settings")]
    [SerializeField] private float _dropRadius = 1.3f;
    [SerializeField] private float _moveAfterDropTime = 0.6f;
    [SerializeField] private float _moveAfterMergeTime = 0.6f;

    internal int Count => _count;
    internal ResourceType Type => _config.Type;

    Vector3 IMergingResource.Position => transform.position;
    int IMergingResource.Count => _count;
    ResourceType IMergingResource.Type => Type;

    internal void Construct(ResourceFactory factory)
    {
        _factory = factory;
    }

    internal void Init(ResourceConfig config)
    {
        _config = config;
        _count = 1;
        _mergeTimer = 0;
        _isMerging = false;

        _view.Init(_config.Sprite);
        _view.ShowCount(_count);
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

    bool IMergingResource.IsReadyToMerge(float timeToMerge)
    {
        return !_isMerging && _mergeTimer >= timeToMerge;
    }

    void IMergingResource.SetCount(int value)
    {
        _count = value;
        
        _view.ShowCount(_count);
    }

    void IMergingResource.UpdateDroppedTime(float deltaTime)
    {
        _mergeTimer += deltaTime;
    }

    void IMergingResource.MoveAfterMerge(Vector3 toPosition)
    {
        if (_mergeCoroutine != null)
            StopCoroutine(_mergeCoroutine);

        _mergeCoroutine = StartCoroutine(MoveAfterMergeCor(toPosition));
    }

    private IEnumerator MoveAfterMergeCor(Vector3 toPosition)
    {
        _collider.enabled = false;
        _isMerging = true;
        _view.ShowStartMerge();

        Vector3 startPosition = transform.position;
        float timer = 0;

        while (timer < _moveAfterMergeTime)
        {
            float t = timer / _moveAfterMergeTime;
            transform.position = Vector3.Slerp(startPosition, toPosition, t * t);
            _view.ShowMerging(t);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = toPosition;
        _view.ShowEndMerge();

        _factory.Recycle(this);
    }
}
