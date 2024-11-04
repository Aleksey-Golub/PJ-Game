using Code.Services;
using UnityEngine;

public class GameAutoSaver : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _autosaveInterval = 30f;

    private ISaveLoadService _saveLoadService;
    private Timer _autoSaveTimer;

    private void Start()
    {
        var saveLoadService = AllServices.Container.Single<ISaveLoadService>();

        Construct(saveLoadService);
    }

    private void Construct(ISaveLoadService saveLoadService)
    {
        _saveLoadService = saveLoadService;

        _autoSaveTimer = new Timer();
        _autoSaveTimer.Elapsed += AutoSaveProgress;

        StartTimer();
    }

    private void Update()
    {
        _autoSaveTimer.OnUpdate(Time.deltaTime);
    }

    private void AutoSaveProgress(Timer timer)
    {
        Logger.Log("[GameAutoSaver] Auto Save Progress");

        _saveLoadService.SaveProgress();
        StartTimer();
    }

    private void StartTimer()
    {
        _autoSaveTimer.Start(_autosaveInterval);
    }
}
