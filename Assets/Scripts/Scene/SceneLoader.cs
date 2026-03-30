using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Signal.Core;

namespace Signal.Scene
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private TransitionOverlay _overlay;

        private bool _isLoading;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void LoadScene(string sceneName, bool isNewSection = false)
        {
            if (_isLoading) return;
            StartCoroutine(LoadSceneRoutine(sceneName, isNewSection));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, bool isNewSection)
        {
            _isLoading = true;

            yield return _overlay.FadeOut();

            AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
            while (!load.isDone)
                yield return null;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.State.CurrentScene = sceneName;

                if (isNewSection)
                    // Slot 0 is reserved for auto-save. Slots 1-4 are manual saves.
                    GameManager.Instance.SaveToSlot(0);
            }

            yield return _overlay.FadeIn();

            _isLoading = false;
        }
    }
}
