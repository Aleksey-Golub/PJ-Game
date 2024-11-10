using Code.Services;
using System;
using UnityEngine;

[SelectionBase]
public class DungeonEntrance : MonoBehaviour
{
    [SerializeField] private DungeonEntranceView _view;
    [SerializeField] private InteractNeed _interactNeed;
    [SerializeField] private Transform _teleportPoint;
    [SerializeField] private float _restoreTime = 15f;
    [SerializeField] private bool _openInStart;

    private Timer _restoreTimer;
    private bool _isForcedOpen;

    public event Action<DungeonEntrance> InteractedByPlayer;

    private void Start()
    {
        var audio = AllServices.Container.Single<IAudioService>();
        var effectFactory = AllServices.Container.Single<IEffectFactory>();

        Construct(audio, effectFactory);
    }

    public void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        _restoreTimer = new Timer();
        _restoreTimer.Changed += OnTimerChanged;

        if (_openInStart)
            _restoreTimer.Start(Constants.EPSILON);

        _view.Construct(audio, effectFactory);
    }

    private void OnDestroy()
    {
        if (_restoreTimer != null)
            _restoreTimer.Changed -= OnTimerChanged;
    }

    private void Update()
    {
        _restoreTimer.OnUpdate(Time.deltaTime);
    }

    internal bool CanInteract(Player player)
    {
        return _isForcedOpen || _interactNeed.CanInteract(player) && _restoreTimer.IsElapsed;
    }

    internal void Interact(Player player)
    {
        TeleportPlayer(player);

        InteractedByPlayer?.Invoke(this);
    }

    internal void ForceOpen()
    {
        _isForcedOpen = true;
        _view.ShowForceOpened();
    }

    internal void ForceClose()
    {
        _isForcedOpen = false;
        _view.ShowForceClosed();
    }

    internal void ReStart()
    {
        _restoreTimer.Start(_restoreTime);
    }

    private void TeleportPlayer(Player player)
    {
        player.Teleport(_teleportPoint.position);
    }

    private void OnTimerChanged(Timer timer)
    {
        _view.ShowProgress(timer.Passed, timer.Duration);
    }
}
