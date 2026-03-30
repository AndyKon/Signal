using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Signal.Narrative
{
    public class NarrativeUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _textDisplay;
        [SerializeField] private float _charDelay = 0.03f;

        private Coroutine _typewriterRoutine;
        private bool _skipRequested;
        private bool _isDisplaying;

        public bool IsDisplaying => _isDisplaying;

        public void Show(string text)
        {
            _panel.SetActive(true);
            _isDisplaying = true;
            _skipRequested = false;

            if (_typewriterRoutine != null)
                StopCoroutine(_typewriterRoutine);

            _typewriterRoutine = StartCoroutine(Typewriter(text));
        }

        public void Hide()
        {
            _panel.SetActive(false);
            _isDisplaying = false;
            _textDisplay.text = "";

            if (_typewriterRoutine != null)
            {
                StopCoroutine(_typewriterRoutine);
                _typewriterRoutine = null;
            }
        }

        public void RequestSkip()
        {
            _skipRequested = true;
        }

        private IEnumerator Typewriter(string fullText)
        {
            _textDisplay.text = "";

            foreach (char c in fullText)
            {
                if (_skipRequested)
                {
                    _textDisplay.text = fullText;
                    break;
                }

                _textDisplay.text += c;
                yield return new WaitForSeconds(_charDelay);
            }

            _typewriterRoutine = null;

            // Wait for click to dismiss
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            Hide();
        }
    }
}
