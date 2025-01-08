using Code.Services;
using UnityEngine;

public class InterstitialAdsRunner : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _showInterval = 360f;

    private IAdsService _adsService;
    private Timer _timer;

#if DEBUG && FAST_DEBUG
    private void Awake() => _showInterval = 20f;
#endif

    private void Start()
    {
        var adsService = AllServices.Container.Single<IAdsService>();

        Construct(adsService);
    }

    private void Construct(IAdsService adsService)
    {
        _adsService = adsService;

        _timer = new Timer();
        _timer.Elapsed += ShowInterstitial;

        StartTimer();
    }

    private void OnDestroy()
    {
        if (_timer != null)
            _timer.Elapsed -= ShowInterstitial;
    }

    private void Update()
    {
        _timer.OnUpdate(Time.deltaTime);
    }

    private void ShowInterstitial(Timer timer)
    {
        Logger.Log("[InterstitialAdsRunner] ShowInterstitial");

        _adsService.ShowFullscreen();
        StartTimer();
    }

    private void StartTimer()
    {
        _timer.Start(_showInterval);
    }
}
