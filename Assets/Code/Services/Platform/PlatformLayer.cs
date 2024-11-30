using UnityEngine;

namespace Code.Services
{
    public class PlatformLayer : MonoBehaviour, IPlatformLayer
    {
        private static bool _isInitialized;

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

        public void Initialize()
        {
#if DEBUG && FAKE_ADS
            _isInitialized = true;
#else
#endif
        }
    }
}
