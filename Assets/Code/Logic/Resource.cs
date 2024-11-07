using Code.Data;
using Code.Services;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Resource : MonoBehaviour, IMergingResource, IPoolable, ISavedProgressWriter, IUniqueIdHolder
{
    [field: SerializeField] public UniqueId UniqueId { get; private set; }
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ResourceView _view;

    private Dropper _dropper;
    private IRecyclableFactory _factory;
    private IAudioService _audio;
    private IPersistentProgressService _progressService;
    private ResourceConfig _config;
    private Coroutine _mergeCoroutine;
    private int _count;
    private float _mergeTimer;
    private bool _isMerging;
    private bool _pickedUp;

    [Header("Settings")]
    [SerializeField] private float _moveAfterMergeTime = 0.6f;

    private string Id => UniqueId.Id;
    public int Count => _count;
    public ResourceType Type => _config.Type;
    public Vector3 Position => transform.position;
    public bool IsConstructed { get; private set; }

    public void Construct(IRecyclableFactory factory, IAudioService audio, IPersistentProgressService progressService)
    {
        _factory = factory;
        _audio = audio;
        _progressService = progressService;

        _dropper = new();

        IsConstructed = true;
    }

    internal void Init(ResourceConfig config, int count)
    {
        _config = config;
        _count = count;
        _mergeTimer = 0;
        _isMerging = false;
        _pickedUp = false;

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
        _pickedUp = true;
        UpdateWorldData();

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
        RemoveResourceFromSavedResources();

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

    void ISavedProgressWriter.WriteToProgress(GameProgress progress)
    {
        if (_pickedUp || _count == 0)
            return;

        var resourcesOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[CurrentLevel()].ResourcesDatas.ResourcesOnScene;

        resourcesOnScene.Dictionary[Id] = new ResourceOnSceneData(transform.position.AsVectorData(), _count, Type);
    }

    private static string CurrentLevel() => SceneManager.GetActiveScene().name;

    private void UpdateWorldData()
    {
        // possible place for pickuper inventory data changing
        RemoveResourceFromSavedResources();
    }

    private void RemoveResourceFromSavedResources()
    {
        ResourcesDataDictionary resourcesOnScene = _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[CurrentLevel()].ResourcesDatas.ResourcesOnScene;

        if (resourcesOnScene.Dictionary.ContainsKey(Id))
            resourcesOnScene.Dictionary.Remove(Id);
    }
}
