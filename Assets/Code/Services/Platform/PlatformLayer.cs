using System;
using UnityEngine;

namespace Code.Services
{
    public class PlatformLayer : MonoBehaviour, IPlatformLayer
    {
#if DEBUG && FAKE_ADS
        private static bool _isInitialized;
#endif

        public static bool IsInitialized
        {
            get
            {
#if FAKE_ADS
                return _isInitialized;
#else
                return GamePush.GP_Init.isReady;
#endif
            }
        }

        public static event Action WebGamePaused;
        public static event Action WebGameResumed;
        public static event Action WebGlWindowClosedOrRefreshed;

        public void Initialize()
        {
#if DEBUG && FAKE_ADS
            _isInitialized = true;
#else
            GamePush.GP_Game.OnPause += () =>
            {
                Logger.Log($"[PlatformLayer] paused");
                WebGamePaused?.Invoke();
            };
            GamePush.GP_Game.OnResume += () =>
            {
                Logger.Log($"[PlatformLayer] resumed");
                WebGameResumed?.Invoke();
            };
#endif
            //Application.focusChanged += (r) => Logger.Log($"[PlatformLayer] focus= {r}");
        }

        public void WindowClosedOrRefreshed()
        {
            Logger.Log("[PlatformLayer] WindowCloseOrRefresh() called");
            WebGlWindowClosedOrRefreshed?.Invoke();
        }
    }
}
