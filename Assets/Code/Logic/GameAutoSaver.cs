using Code.Services;
using UnityEngine;

public class GameAutoSaver : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _autosaveInterval = 30f;

    private ISaveLoadService _saveLoadService;
    private Timer _autoSaveTimer;

#if DEBUG && FAST_DEBUG
    private void Awake() => _autosaveInterval = 2f;
#endif

    private void Start()
    {
        var saveLoadService = AllServices.Container.Single<ISaveLoadService>();

        Construct(saveLoadService);
    }

    private void Construct(ISaveLoadService saveLoadService)
    {
        _saveLoadService = saveLoadService;

        PlatformLayer.WebGameResumed += OnGameResumed;
        PlatformLayer.WebGlWindowClosedOrRefreshed += OnWebGlWindowClosedOrRefreshed;

        _autoSaveTimer = new Timer();
        _autoSaveTimer.Elapsed += AutoSaveProgress;

        StartTimer();
    }

    private void OnDestroy()
    {
        PlatformLayer.WebGameResumed -= OnGameResumed;
        PlatformLayer.WebGlWindowClosedOrRefreshed -= OnWebGlWindowClosedOrRefreshed;
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

    private void SaveProgress()
    {
        Logger.Log("[GameAutoSaver] Save Progress");

        _saveLoadService.SaveProgress();
    }

    private void StartTimer()
    {
        _autoSaveTimer.Start(_autosaveInterval);
    }

    private void OnGameResumed() => SaveProgress();

    private void OnWebGlWindowClosedOrRefreshed()
    {
#if DEBUG
        Logger.LogWarning($"[GameAutoSaver] On WebGlWindowClosedOrRefreshed save progress disabled");
#else
        SaveProgress();
#endif
    }
}
