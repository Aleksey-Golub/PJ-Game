using System.Collections;
using UnityEngine;

namespace Code.Infrastructure
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private float _stepAndDelay = 0.03f;
        [SerializeField] private CanvasGroup _curtain;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _curtain.alpha = 1;
        }

        public void Hide() => StartCoroutine(DoFadeIn());

        private IEnumerator DoFadeIn()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(_stepAndDelay);
            
            while (_curtain.alpha > 0)
            {
                _curtain.alpha -= _stepAndDelay;
                yield return waitForSeconds;
            }

            gameObject.SetActive(false);
        }
    }
}