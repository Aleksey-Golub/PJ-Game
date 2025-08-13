using Code.Services;
using Code.UI.Services;
using System.Collections;
using UnityEngine;

/// <summary>
/// Show Interstitial ads and PREMIUM ads
/// </summary>
public class InterstitialAdsAndPremiumRunner : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _showInterval = 240f;
    [SerializeField, Min(1f)] private float _showPremiumInterval = 600f;

    private IAdsService _adsService;
    private IIAPService _iapService;
    private IUIMediator _uiMediator;
    private Timer _showInterstitialTimer;

    private Timer _showPremiumTimer;
    private Coroutine _showPremiumCoroutine;
    private readonly WaitForSeconds _wait = new WaitForSeconds(5f);

#if DEBUG && FAST_DEBUG
    private void Awake() => _showInterval = 20f;
    private void Awake() => _showPremiumInterval = 50f;
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

        StartInterstitialTimer();
        StartPremiumTimer();
    }

    private void OnDestroy()
    {
        if (_showInterstitialTimer != null)
            _showInterstitialTimer.Elapsed -= ShowInterstitial;
        
        if (_showPremiumTimer != null)
            _showPremiumTimer.Elapsed -= ShowPremium;
    }

    private void Update()
    {
        _showInterstitialTimer.OnUpdate(Time.deltaTime);
        _showPremiumTimer.OnUpdate(Time.deltaTime);
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

    private void StartInterstitialTimer() => _showInterstitialTimer.Start(_showInterval);
    private void StartPremiumTimer() => _showPremiumTimer.Start(_showPremiumInterval);
}
