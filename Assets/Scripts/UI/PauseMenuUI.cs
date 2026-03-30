using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Signal.Core;

namespace Signal.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _quitToMenuButton;
        [SerializeField] private SaveSlotUI _saveSlotUI;
        [SerializeField] private string _mainMenuScene = "MainMenu";

        private bool _isPaused;

        private void Start()
        {
            _resumeButton.onClick.AddListener(Resume);
            _saveButton.onClick.AddListener(OnSave);
            _quitToMenuButton.onClick.AddListener(OnQuitToMenu);
            _pausePanel.SetActive(false);
            _saveSlotUI.Hide();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused) Resume();
                else Pause();
            }
        }

        private void Pause()
        {
            _isPaused = true;
            _pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }

        private void Resume()
        {
            _isPaused = false;
            _pausePanel.SetActive(false);
            _saveSlotUI.Hide();
            Time.timeScale = 1f;
        }

        private void OnSave()
        {
            _saveSlotUI.Show(OnSaveSlotSelected, showEmpty: true);
        }

        private void OnSaveSlotSelected(int slot)
        {
            GameManager.Instance.SaveToSlot(slot);
        }

        private void OnQuitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_mainMenuScene);
        }
    }
}
