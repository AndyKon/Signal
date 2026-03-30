using System;
using System.Collections.Generic;
using UnityEngine;
using Signal.Core;

namespace Signal.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [SerializeField] private List<ItemDefinition> _allItems = new();

        public event Action OnInventoryChanged;

        private readonly Dictionary<string, ItemDefinition> _itemLookup = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var item in _allItems)
                _itemLookup[item.ItemId] = item;
        }

        public void AddItem(string itemId)
        {
            GameManager.Instance.State.AddItem(itemId);
            OnInventoryChanged?.Invoke();
        }

        public void RemoveItem(string itemId)
        {
            GameManager.Instance.State.RemoveItem(itemId);
            OnInventoryChanged?.Invoke();
        }

        public bool HasItem(string itemId) => GameManager.Instance.State.HasItem(itemId);

        public ItemDefinition GetDefinition(string itemId) =>
            _itemLookup.TryGetValue(itemId, out var def) ? def : null;

        public List<ItemDefinition> GetHeldItemDefinitions()
        {
            var result = new List<ItemDefinition>();
            foreach (string id in GameManager.Instance.State.Inventory)
            {
                if (_itemLookup.TryGetValue(id, out var def))
                    result.Add(def);
            }
            return result;
        }
    }
}
