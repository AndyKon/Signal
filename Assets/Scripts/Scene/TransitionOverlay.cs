using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Signal.Scene
{
    public class TransitionOverlay : MonoBehaviour
    {
        [SerializeField] private Image _overlay;
        [SerializeField] private float _fadeDuration = 0.5f;

        public float FadeDuration => _fadeDuration;

        public IEnumerator FadeOut()
        {
            yield return Fade(0f, 1f);
        }

        public IEnumerator FadeIn()
        {
            yield return Fade(1f, 0f);
        }

        private IEnumerator Fade(float from, float to)
        {
            float elapsed = 0f;
            Color color = _overlay.color;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(from, to, elapsed / _fadeDuration);
                _overlay.color = color;
                yield return null;
            }

            color.a = to;
            _overlay.color = color;
        }
    }
}
