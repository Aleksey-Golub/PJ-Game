using UnityEngine;
using Code.Services;
using Code.Data;
using Code.Infrastructure;
using System.Collections;

public class Tutorial : MonoBehaviour, ISavedProgressReader, ISavedProgressWriter
{
    [SerializeField] private TutorialConfig _config;
    
    private IGameFactory _gameFactory;
    private IPersistentProgressService _progressService;
    private Player _player;

    private TutorialStage _currentStage;
    private bool _isCompleted;
    private Timer _timer;
    private float _lastTime = 0;

    public void Construct(IGameFactory gameFactory, IPersistentProgressService progressService, Player player)
    {
        _gameFactory = gameFactory;
        _progressService = progressService;
        _player = player;

        _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed += OnUpgradeItemsProgress_Changed;

        _player.Inventory.ResourceAdded += OnPlayerInventory_ResourceAdded;
        _player.Inventory.ResourceRemoveed += OnPlayerInventory_ResourceRemoveed;
        _player.Inventory.ToolAdded += OnPlayerInventory_ToolAdded;
    
        _timer = new Timer();
        _timer.Changed += OnTimerChanged;
        _timer.Start(float.MaxValue);
    }

    private void Update()
    {
        _timer.OnUpdate(Time.deltaTime);
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    private void CleanUp()
    {
        _progressService.Progress.PlayerProgress.UpgradeItemsProgress.Changed -= OnUpgradeItemsProgress_Changed;

        _player.Inventory.ResourceAdded -= OnPlayerInventory_ResourceAdded;
        _player.Inventory.ResourceRemoveed -= OnPlayerInventory_ResourceRemoveed;
        _player.Inventory.ToolAdded -= OnPlayerInventory_ToolAdded;

        _timer.Changed -= OnTimerChanged;
    }

    public void ReadProgress(GameProgress progress)
    {
        var myState = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].TutorialProgress;

        if (myState.IsCompleted)
        {
            Logger.LogWarning($"[Tutorial] ReadProgress() on scene {SceneLoader.CurrentLevel()} while it is complite");
            return;
        }

        // restore state
        int currentStageNumer = myState.Stage;
        int[] goalsProgresses = myState.GoalsProgresses;

        if (currentStageNumer == 0)
        {
            DoFirstStart();
            return;
        }

        _currentStage = new TutorialStage(_config.Stages.Find(s => s.Number == currentStageNumer));
        _currentStage.RestoreProgress(goalsProgresses);
    }

    public void WriteToProgress(GameProgress progress)
    {
        var myState = progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].TutorialProgress;

        myState.Stage = _currentStage.Number;
        myState.IsCompleted = _isCompleted;
        myState.GoalsProgresses = _currentStage.GetGoalsProgresses();
    }

    private void DoFirstStart()
    {
        StartCoroutine(EnterFirstStateCor());

        IEnumerator EnterFirstStateCor()
        {
            yield return null;
            SwitchStage(1);
        }
    }

    private void OnTimerChanged(Timer timer)
    {
        while (timer.Passed - _lastTime > 1)
        {
            _lastTime += 1;
            _currentStage.OnOneSecondPassed();
            CheckStageOrTutorialEnded();
        }
    }

    private void OnUpgradeItemsProgress_Changed(string itemId, int newLevel)
    {
        _currentStage.OnUpgradeItemsProgress_Changed(itemId, newLevel);
        CheckStageOrTutorialEnded();
    }

    private void OnPlayerInventory_ResourceAdded(ResourceType type, int addedCount)
    {
        _currentStage.OnPlayerInventory_ResourceAdded(type, addedCount);
        CheckStageOrTutorialEnded();
    }

    private void OnPlayerInventory_ResourceRemoveed(ResourceType type, int removedCount)
    {
        _currentStage.OnPlayerInventory_ResourceRemoved(type, removedCount);
        CheckStageOrTutorialEnded();
    }

    private void OnPlayerInventory_ToolAdded(ToolType type)
    {
        _currentStage.OnPlayerInventory_ToolAdded(type);
        CheckStageOrTutorialEnded();
    }

    private void CheckStageOrTutorialEnded()
    {
        if (_currentStage.IsComplited && _currentStage.IsFinal)
        {
            _isCompleted = true;
            WriteToProgress(_progressService.Progress);

            Destroy(gameObject);

            return;
        }

        if (_currentStage.IsComplited)
            SwitchStage(_currentStage.NextStageNumber);
    }

    private void SwitchStage(int nextStageNumber)
    {
        // exit prev
        _currentStage = new TutorialStage(_config.Stages.Find(s => s.Number == nextStageNumber));
        _currentStage.Enter(this);

        CheckStageOrTutorialEnded();
    }

    internal void Handle(TutorialEventSpawnObjectWithId spawnEvent)
    {
        GameObject go = _gameFactory.GetGameObject(spawnEvent.SpawnedGameObjectId, spawnEvent.Position);
        go.GetComponent<IUniqueIdHolder>().UniqueId.Id = spawnEvent.AssignedId;
    }

    internal void Handle(TutorialEventDestroyObjectWithId destoyEvent)
    {
        GameObject go = null;
        foreach (ISavedProgressWriter item in _gameFactory.ProgressWriters)
        {
            if (item is IUniqueIdHolder uniqueIdHolder && uniqueIdHolder.UniqueId.Id == destoyEvent.UniqueAssignedId)
            {
                go = uniqueIdHolder.UniqueId.gameObject;
            }
        }

        if (go != null)
        {
            // TODO
            // introduce Interface and use Interface to command go to delete its progress itself
            _progressService.Progress.WorldProgress.LevelsDatasDictionary.Dictionary[SceneLoader.CurrentLevel()].TutorialProgress.TutorialObjectsDatas.TutorialObjectsOnScene.Dictionary.Remove(destoyEvent.UniqueAssignedId);
            _gameFactory.Recycle(go);
        }
        else
            Logger.LogWarning($"[Tutorial] try to destroy absent GameObject, uniqueId= {destoyEvent.UniqueAssignedId}");
    }
}
