using Code.Services;
using Code.UI.Services;
using System.Collections;
using UnityEngine;

/// <summary>
/// Show Interstitial ads, PREMIUM ads, Support ads
/// </summary>
public class InterstitialAdsAndPremiumRunner : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _showInterval = 240f;
    [SerializeField, Min(1f)] private float _showPremiumInterval = 600f;
    [SerializeField, Min(1f)] private float _showSupportInterval = 720f;

    private IAdsService _adsService;
    private IIAPService _iapService;
    private IUIMediator _uiMediator;
    private Timer _showInterstitialTimer;

    private Timer _showPremiumTimer;
    private Coroutine _showPremiumCoroutine;
    private Timer _showSupportTimer;
    private Coroutine _showSupportCoroutine;
    private readonly WaitForSeconds _wait = new WaitForSeconds(5f);

#if DEBUG && FAST_DEBUG_ADS_INTERVAL
    private void Awake() => _showInterval = 20f;
    private void Awake() => _showPremiumInterval = 50f;
    private void Awake() => _showSupportInterval = 60f;
#endif

    private void Start()
    {
        var adsService = AllServices.Container.Single<IAdsService>();
        var iapService = AllServices.Container.Single<IIAPService>();
        var uiMediator = AllServices.Container.Single<IUIMediator>();

        Construct(adsService, iapService, uiMediator);
    }

    private void Construct(IAdsService adsService, IIAPService iapService, IUIMediator uiMediator)
    {
        _adsService = adsService;
        _iapService = iapService;
        _uiMediator = uiMediator;

        _showInterstitialTimer = new Timer();
        _showInterstitialTimer.Elapsed += ShowInterstitial;
        
        _showPremiumTimer = new Timer();
        _showPremiumTimer.Elapsed += ShowPremium;

        _showSupportTimer = new Timer();
        _showSupportTimer.Elapsed += ShowSupport;

        StartInterstitialTimer();
        StartPremiumTimer();
        StartSupportTimer();
    }

    private void OnDestroy()
    {
        if (_showInterstitialTimer != null)
            _showInterstitialTimer.Elapsed -= ShowInterstitial;
        
        if (_showPremiumTimer != null)
            _showPremiumTimer.Elapsed -= ShowPremium;
        
        if (_showSupportTimer != null)
            _showSupportTimer.Elapsed -= ShowSupport;
    }

    private void Update()
    {
        _showInterstitialTimer.OnUpdate(Time.deltaTime);
        _showPremiumTimer.OnUpdate(Time.deltaTime);
        _showSupportTimer.OnUpdate(Time.deltaTime);
    }

    private void ShowInterstitial(Timer timer)
    {
        Logger.Log("[InterstitialAdsRunner] ShowInterstitial");

        _adsService.ShowFullscreen();
        StartInterstitialTimer();
    }
    
    private void ShowPremium(Timer timer)
    {
        if (_iapService.IsPremiumBought())
            return;

        if (_showPremiumCoroutine != null)
            StopCoroutine(_showPremiumCoroutine);
    
        _showPremiumCoroutine = StartCoroutine(ShowPremium());
    }

    private IEnumerator ShowPremium()
    {
        while (_adsService.IsFullscreenShowing)
            yield return _wait;

        Logger.Log("[InterstitialAdsRunner] ShowPremium");

        _uiMediator.Open(WindowId.BuyPremium);
        StartPremiumTimer();
    }
    
    private void ShowSupport(Timer timer)
    {
        if (_iapService.IsSupportBought())
            return;

        if (_showSupportCoroutine != null)
            StopCoroutine(_showSupportCoroutine);

        _showSupportCoroutine = StartCoroutine(ShowSupport());
    }

    private IEnumerator ShowSupport()
    {
        while (_adsService.IsFullscreenShowing)
            yield return _wait;

        Logger.Log("[InterstitialAdsRunner] ShowSupport");

        _uiMediator.Open(WindowId.BuySupport);
        StartSupportTimer();
    }

    private void StartInterstitialTimer() => _showInterstitialTimer.Start(_showInterval);
    private void StartPremiumTimer() => _showPremiumTimer.Start(_showPremiumInterval);
    private void StartSupportTimer() => _showSupportTimer.Start(_showSupportInterval);
}
