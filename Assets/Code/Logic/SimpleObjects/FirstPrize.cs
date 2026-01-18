using Code.Infrastructure;
using Code.Services;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FirstPrize : SimpleObjectBase, ICreatedByIdGameObject
{
    [SerializeField] private SimpleObjectType _type;
    [SerializeField] private FirstPrizeView _view;
    [SerializeField] private SpawnGameObjectData[] _spawnDatas;
    private IAudioService _audio;
    private IGameFactory _gameFactory;
    private IPersistentProgressService _progressService;
    private IPlayerProvider _playerProvider;
    private bool _playerInTrigger;

    protected override SimpleObjectType Type => _type;

    #region EDITOR
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        foreach (SpawnGameObjectData data in _spawnDatas)
        {
            Vector3 pos = data.Point != null ? data.Point.position : data.Position;
            Handles.Label(pos, data.GameObjectId);
        }
    }
#endif
    #endregion

    private void Start()
    {
        if (SceneBuiltInItem)
        {
            var audio = AllServices.Container.Single<IAudioService>();
            var effectFactory = AllServices.Container.Single<IEffectFactory>();
            var gameFactory = AllServices.Container.Single<IGameFactory>();
            var progressService = AllServices.Container.Single<IPersistentProgressService>();
            var playerProvider = AllServices.Container.Single<IPlayerProvider>();

            Construct(audio, effectFactory, gameFactory, progressService, playerProvider);
            gameFactory.RegisterProgressWatchersExternal(gameObject);
        }
    }

    internal void Construct(
        IAudioService audio, 
        IEffectFactory effectFactory,
        IGameFactory gameFactory, 
        IPersistentProgressService progressService, 
        IPlayerProvider playerProvider
        )
    {
        _audio = audio;
        _gameFactory = gameFactory;
        _progressService = progressService;
        _playerProvider = playerProvider;

        _view.Construct(audio, effectFactory);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out Player player))
            return;

        if (_playerInTrigger)
            return;

        _playerInTrigger = true;

        _view.ShowInteract();
        _view.PlayInteractSound();

        StartCoroutine(DoSequence());
    }

    private IEnumerator DoSequence()
    {
        var player = _playerProvider.GetPlayer();

        player.BlockMovement();
        _audio.PauseAmbient();
        yield return _view.PlayCutScene();
        SpawnPortal();
        RemoveSelf();
        player.UnBlockMovement();
        _audio.UnPauseAmbient();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out Player player))
            return;

        if (!_playerInTrigger)
            return;

        _playerInTrigger = false;

        _view.HideInteract();
        _view.StopInteractSound();
    }

    private void SpawnPortal()
    {
        _view.PlaySpawnPortalSound();
        _view.ShowSpawnPortalEffect();

        foreach (SpawnGameObjectData data in _spawnDatas)
            _gameFactory.GetGameObject(data.GameObjectId, at: data.Point != null ? data.Point.position : data.Position);
    }

    private void RemoveSelf()
    {
        if (SceneBuiltInItem)
        {
            gameObject.SetActive(false);
            return;
        }

        _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].SimpleObjectsDatas.SimpleObjectsOnScene.Dictionary.Remove(UniqueId.Id);
        _gameFactory.Recycle(gameObject);
    }

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => visitor.Visit(this);
}
