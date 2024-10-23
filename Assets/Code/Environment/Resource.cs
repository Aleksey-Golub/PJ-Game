using System.Collections;
using UnityEngine;

internal class Resource : MonoBehaviour, IMergingResource, IPoolable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ResourceView _view;

    private Dropper _dropper;
    private IRecyclableFactory _factory;
    private AudioService _audio;
    private ResourceConfig _config;
    private Coroutine _mergeCoroutine;
    private int _count;
    private float _mergeTimer;
    private bool _isMerging;

    [Header("Settings")]
    [SerializeField] private float _moveAfterMergeTime = 0.6f;

    public int Count => _count;
    public ResourceType Type => _config.Type;
    public Vector3 Position => transform.position;

    void IPoolable.Construct(IRecyclableFactory factory, AudioService audio)
    {
        _factory = factory;
        _audio = audio;
        _dropper = new();
    }

    internal void Init(ResourceConfig config, int count)
    {
        _config = config;
        _count = count;
        _mergeTimer = 0;
        _isMerging = false;

        _view.Init(_config.Sprite);
        _view.ShowCount(_count);
        _collider.enabled = false;
    }

    internal void MoveAfterDrop(DropData dropData)
    {
        _dropper.MoveAfterDrop(this, _view, _collider, dropData);
    }

    internal void Collect()
    {
        _audio.PlaySfxAtPosition(_config.PickupAudio, transform.position);

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
