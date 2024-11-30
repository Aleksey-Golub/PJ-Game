using Code.Services;
using UnityEngine;

public abstract class AdsObjectBase<T> : SimpleObjectBase, ICreatedByIdGameObject where T : AdsObjectView
{
    [SerializeField] private SimpleObjectType _type;
    [SerializeField] protected T View;
    [SerializeField] private float _adsStartDelay = 3f;
    [SerializeField] private float _restoreTime = 180f;

    private IAdsService _adsService;

    private bool _playerInTrigger;
    private Timer _adsTimer;
    private Timer _restorationTimer;
    private bool _isExhaust;

    protected Player Player;

    protected override SimpleObjectType Type => _type;

    protected void Construct(IAdsService adsService, IAudioService audio)
    {
        _adsService = adsService;
        _adsService.RewardedVideoReady += OnRewardedVideoReady;

        _adsTimer = new Timer();
        _adsTimer.Changed += OnAdsTimerChanged;

        _restorationTimer = new Timer();
        _restorationTimer.Elapsed += OnRestorationTimerElapsed;

        View.Construct(audio);

        if (!AdsReady())
        {
            _isExhaust = true;

            _restorationTimer.Start(1f);
            View.ShowExhaust();
        }
    }

    private void OnDestroy()
    {
        if (_adsTimer != null)
            _adsTimer.Changed -= OnAdsTimerChanged;

        if (_restorationTimer != null)
            _restorationTimer.Elapsed -= OnRestorationTimerElapsed;

        if (_adsService != null)
            _adsService.RewardedVideoReady -= OnRewardedVideoReady;
    }

    private void Update() => OnUpdate(Time.deltaTime);
    private void OnTriggerEnter2D(Collider2D collision) => TriggerEnterHandler(collision);
    private void OnTriggerStay2D(Collider2D collision) => TriggerStayHandler(collision);
    private void OnTriggerExit2D(Collider2D collision) => TriggerExitHandler(collision);

    protected virtual void OnRewardedVideoEndSuccessfully()
    {
        View.PlayRewardAcceptSound();
    }

    private void TriggerEnterHandler(Collider2D collision)
    {
        if (_isExhaust)
            return;

        if (!collision.gameObject.TryGetComponent(out Player player))
            return;

        if (_playerInTrigger)
            return;

        _playerInTrigger = true;

        if (!AdsReady())
            return;

        _adsTimer.Start(_adsStartDelay);

        Player = player;
    }

    private void TriggerStayHandler(Collider2D collision)
    {
        if (_isExhaust)
            return;

        if (!collision.gameObject.TryGetComponent(out Player player))
            return;

        if (!_adsTimer.IsStarted)
            return;

        _adsTimer.OnUpdate(Time.deltaTime);

        if (_adsTimer.IsElapsed)
        {
            //Logger.Log($"[AdsObject] {gameObject.name} - show ads");

            View.HideProgress();
            Exhaust();
            _adsService.ShowRewardedVideo(OnRewardedVideoEndSuccessfully);
        }
    }

    private void TriggerExitHandler(Collider2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out Player player))
            return;

        if (!_playerInTrigger)
            return;

        _playerInTrigger = false;

        View.HideProgress();
    }

    private void OnUpdate(float deltaTime)
    {
        if (!_isExhaust)
            return;

        _restorationTimer.OnUpdate(deltaTime);
    }

    private void OnRewardedVideoReady()
    {
        if (_restorationTimer.IsElapsed && _isExhaust)
            Restore();
    }

    private void OnRestorationTimerElapsed(Timer timer)
    {
        if (AdsReady())
            Restore();
    }

    private void Restore()
    {
        _isExhaust = false;
        View.ShowWhole();
    }

    private void OnAdsTimerChanged(Timer timer)
    {
        View.ShowProgress(timer.Passed, _adsTimer.Duration);
    }

    private void Exhaust()
    {
        _isExhaust = true;

        _restorationTimer.Start(_restoreTime);
        View.ShowExhaust();
    }

    private bool AdsReady() => _adsService.IsRewardedVideoReady();

    void ICreatedByIdGameObject.Accept(ICreatedByIdGameObjectVisitor visitor) => Accept(visitor);
    protected abstract void Accept(ICreatedByIdGameObjectVisitor visitor);
}
