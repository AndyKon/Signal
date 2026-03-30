using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Signal.Core;

namespace Signal.UI
{
    public class SaveSlotUI : MonoBehaviour
    {
        [SerializeField] private Transform _slotContainer;
        [SerializeField] private GameObject _slotButtonPrefab;

        private Action<int> _onSlotSelected;

        public void Show(Action<int> onSlotSelected, bool showEmpty = true)
        {
            _onSlotSelected = onSlotSelected;
            gameObject.SetActive(true);
            PopulateSlots(showEmpty);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void PopulateSlots(bool showEmpty)
        {
            foreach (Transform child in _slotContainer)
                Destroy(child.gameObject);

            var saveSystem = GameManager.Instance.SaveSystem;

            for (int i = 0; i < 5; i++)
            {
                bool exists = saveSystem.SlotExists(i);
                if (!exists && !showEmpty) continue;

                int slotIndex = i;
                GameObject button = Instantiate(_slotButtonPrefab, _slotContainer);
                var text = button.GetComponentInChildren<TextMeshProUGUI>();

                if (exists)
                {
                    SaveData data = saveSystem.Load(i);
                    text.text = $"Slot {i + 1}: {data.CurrentScene}";
                }
                else
                {
                    text.text = $"Slot {i + 1}: Empty";
                }

                button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    _onSlotSelected?.Invoke(slotIndex);
                    Hide();
                });
            }
        }
    }
}
