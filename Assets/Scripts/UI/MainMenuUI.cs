using UnityEngine;
using UnityEngine.UI;
using Signal.Core;
using Signal.Scene;

namespace Signal.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private SaveSlotUI _saveSlotUI;
        [SerializeField] private string _firstSceneName = "Section1_Hub_Room1";

        private void Start()
        {
            _newGameButton.onClick.AddListener(OnNewGame);
            _loadGameButton.onClick.AddListener(OnLoadGame);
            _quitButton.onClick.AddListener(OnQuit);
            _saveSlotUI.Hide();
        }

        private void OnNewGame()
        {
            Debug.Log("[MainMenu] New Game clicked");

            if (GameManager.Instance == null)
            {
                Debug.LogError("[MainMenu] GameManager.Instance is null — Bootstrap may have failed");
                return;
            }

            if (SceneLoader.Instance == null)
            {
                Debug.LogError("[MainMenu] SceneLoader.Instance is null");
                return;
            }

            GameManager.Instance.NewGame();
            Debug.Log($"[MainMenu] Loading scene: {_firstSceneName}");
            SceneLoader.Instance.LoadScene(_firstSceneName, isNewSection: true);
        }

        private void OnLoadGame()
        {
            _saveSlotUI.Show(OnLoadSlotSelected, showEmpty: false);
        }

        private void OnLoadSlotSelected(int slot)
        {
            if (GameManager.Instance.LoadFromSlot(slot))
            {
                string scene = GameManager.Instance.State.CurrentScene;
                SceneLoader.Instance.LoadScene(scene);
            }
        }

        private void OnQuit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
