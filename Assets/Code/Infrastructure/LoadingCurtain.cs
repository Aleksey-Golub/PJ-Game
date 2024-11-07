using System.Collections;
using UnityEngine;

namespace Code.Infrastructure
{
    public class LoadingCurtain : MonoBehaviour
    {
        [SerializeField] private float _stepAndDelay = 0.03f;
        [SerializeField] private AnimationCurve _animationCurve;
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
            float t = 0;

            while (t < 1)
            {
                _curtain.alpha = _animationCurve.Evaluate(t);
                t += _stepAndDelay;
                yield return waitForSeconds;
            }

            _curtain.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}