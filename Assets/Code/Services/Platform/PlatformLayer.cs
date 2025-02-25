﻿using System;
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

        public static string PlatformName
        {
            get
            {
#if DEBUG && FAKE_ADS
                return "Not GamePush";
#else
                string device = GamePush.GP_Device.IsMobile() ? "mobile" : "desktop";
                return $"{GamePush.GP_Platform.Type().ToString()} {device}";
#endif
            }
        }

        public static event Action WebGamePaused;
        public static event Action WebGameResumed;
        public static event Action WebGlWindowClosedOrRefreshed;
        public static event Action WebGlDocumentVisibilitySetToHidden;

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

        /// <summary>
        /// Call this when game fully available to player: all progress bars, downloads etc ended; MainMenu or level avalable.
        /// </summary>
        public static void SetGameReady()
        {
#if DEBUG && FAKE_ADS
#else
            GamePush.GP_Game.GameReady();
#endif
        }

        public void WindowClosedOrRefreshed()
        {
            Logger.Log("[PlatformLayer] WindowCloseOrRefresh() called");
            WebGlWindowClosedOrRefreshed?.Invoke();
        }

        public void DocumentVisibilitySetToHidden()
        {
            Logger.Log("[PlatformLayer] DocumentVisibilitySetToHidden() called");
            WebGlDocumentVisibilitySetToHidden?.Invoke();
        }
    }
}
