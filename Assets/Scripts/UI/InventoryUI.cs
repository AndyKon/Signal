using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Signal.Inventory;

namespace Signal.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryBar;
        [SerializeField] private Transform _slotContainer;
        [SerializeField] private GameObject _itemSlotPrefab;
        [SerializeField] private KeyCode _toggleKey = KeyCode.Tab;

        private bool _isVisible;

        private void Start()
        {
            _inventoryBar.SetActive(false);
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnInventoryChanged += Refresh;
        }

        private void OnDestroy()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnInventoryChanged -= Refresh;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                _isVisible = !_isVisible;
                _inventoryBar.SetActive(_isVisible);
                if (_isVisible) Refresh();
            }
        }

        private void Refresh()
        {
            foreach (Transform child in _slotContainer)
                Destroy(child.gameObject);

            List<ItemDefinition> items = InventoryManager.Instance.GetHeldItemDefinitions();
            foreach (var item in items)
            {
                GameObject slot = Instantiate(_itemSlotPrefab, _slotContainer);
                var image = slot.GetComponentInChildren<Image>();
                if (image != null && item.Icon != null)
                    image.sprite = item.Icon;
            }
        }
    }
}
