using UnityEngine;
using Signal.Core;

namespace Signal.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    public class Hotspot : MonoBehaviour
    {
        [SerializeField] private HotspotCondition _condition = new();
        [SerializeField] private HotspotAction _action = new();
        [SerializeField] private HotspotAction _altAction = new();
        [SerializeField] private string _altConditionFlag = "";

        [Header("Visual")]
        [SerializeField] private SpriteRenderer _highlight;

        public bool IsAvailable()
        {
            var state = GameManager.Instance.State;

            if (!string.IsNullOrEmpty(_condition.RequiredFlag) && !state.HasFlag(_condition.RequiredFlag))
                return false;
            if (!string.IsNullOrEmpty(_condition.RequiredItem) && !state.HasItem(_condition.RequiredItem))
                return false;
            if (!string.IsNullOrEmpty(_condition.BlockedByFlag) && state.HasFlag(_condition.BlockedByFlag))
                return false;

            return true;
        }

        public HotspotAction GetAction()
        {
            if (!string.IsNullOrEmpty(_altConditionFlag) && GameManager.Instance.State.HasFlag(_altConditionFlag))
                return _altAction;
            return _action;
        }

        public void SetHighlight(bool on)
        {
            if (_highlight != null)
                _highlight.enabled = on;
        }
    }
}
