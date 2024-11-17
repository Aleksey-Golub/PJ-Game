using Code.Data;
using Code.Services;
using System;
using UnityEngine;

[SelectionBase]
public class DungeonEntrance : MonoBehaviour, ISaveState<DungeonEntranceOnScene>, IRestoreState<DungeonEntranceOnScene>
{
    [SerializeField] private DungeonEntranceView _view;
    [SerializeField] private InteractNeed _interactNeed;
    [SerializeField] private Transform _teleportPoint;
    [SerializeField] private float _restoreTime = 15f;
    [SerializeField] private bool _openInStart;

    private Timer _restoreTimer;
    private bool _isForcedOpen;

    public event Action<DungeonEntrance> InteractedByPlayer;

    public void Construct(IAudioService audio, IEffectFactory effectFactory)
    {
        _restoreTimer = new Timer(_openInStart);
        _restoreTimer.Changed += OnTimerChanged;
        _restoreTimer.Elapsed += OnTimerElapsed;

        _view.Construct(audio, effectFactory);

        if (_openInStart)
            _view.HideProgress();
    }

    private void OnDestroy()
    {
        if (_restoreTimer != null)
        {
            _restoreTimer.Changed -= OnTimerChanged;
            _restoreTimer.Elapsed -= OnTimerElapsed;
        }
    }

    private void Update()
    {
        _restoreTimer.OnUpdate(Time.deltaTime);
    }

    public DungeonEntranceOnScene SaveState()
    {
        return new DungeonEntranceOnScene(
            _isForcedOpen,
            new TimerData(_restoreTimer.IsStarted, _restoreTimer.Passed, _restoreTimer.Duration)
            );
    }

    public void RestoreState(DungeonEntranceOnScene state)
    {
        _isForcedOpen = state.IsForcedOpen;
        if (_isForcedOpen)
            _view.ShowForceOpened();

        if (state.TimerData.IsStarted)
        {
            _restoreTimer.StartAsPartialPassed(state.TimerData.Duration, state.TimerData.Passed);
        }
        else
        {
            if (_openInStart)
                _view.HideProgress();
        }
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
        _view.ShowOpenEffect();
        _view.PlayOpenSound();
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

    private void OnTimerElapsed(Timer timer)
    {
        OnTimerChanged(timer);

        _view.ShowOpenEffect();
        _view.PlayOpenSound();
    }
}
