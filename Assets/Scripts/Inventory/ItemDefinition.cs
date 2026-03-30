using UnityEngine;

namespace Signal.Inventory
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Signal/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        public string ItemId;
        public string DisplayName;
        [TextArea(2, 4)]
        public string Description;
        public Sprite Icon;
    }
}
