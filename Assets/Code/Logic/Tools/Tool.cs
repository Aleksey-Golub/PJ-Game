using Code.Data;
using Code.Infrastructure;
using Code.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tool : MonoBehaviour, IPoolable, ISavedProgressWriter, IUniqueIdHolder
{
    [field: SerializeField] public UniqueId UniqueId { get; private set; }
    [SerializeField] private Collider2D _collider;
    [SerializeField] private ToolView _view;

    private Dropper _dropper;
    private ToolConfig _config;
    private IRecyclableFactory _factory;
    private IAudioService _audio;
    private IPersistentProgressService _progressService;
    private bool _pickedUp;

    public bool IsConstructed { get; private set; }
    private string Id => UniqueId.Id;
    internal ToolType Type => _config.Type;

    public void Construct(IRecyclableFactory factory, IAudioService audio, IPersistentProgressService progressService)
    {
        _factory = factory;
        _audio = audio;
        _progressService = progressService;

        _dropper = new();

        IsConstructed = true;
    }

    internal void Init(ToolConfig config)
    {
        _config = config;
        _pickedUp = false;

        _view.Init(_config.Sprite);
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

    void ISavedProgressWriter.WriteToProgress(GameProgress progress)
    {
        if (_pickedUp)
            return;

        var toolsOnScene = progress.WorldProgress.LevelsDatasDictionary.Dictionary[CurrentLevel()].ToolsDatas.ToolsOnScene;

        toolsOnScene.Dictionary[Id] = new ToolOnSceneData(transform.position.AsVectorData(), Type);
    }

    private string CurrentLevel() => SceneLoader.CurrentLevel();
    
    private void UpdateWorldData()
    {
        // possible place for pickuper inventory data changing
        RemoveToolFromSavedTools();
    }

    private void RemoveToolFromSavedTools()
    {
        var toolsOnScene = _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[CurrentLevel()].ToolsDatas.ToolsOnScene;

        if (toolsOnScene.Dictionary.ContainsKey(Id))
            toolsOnScene.Dictionary.Remove(Id);
    }
}

