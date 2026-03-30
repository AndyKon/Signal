using UnityEngine;
using Signal.Core;
using Signal.Scene;
using Signal.Narrative;
using Signal.Inventory;

namespace Signal.Interaction
{
    public class InteractionManager : MonoBehaviour
    {
        public static InteractionManager Instance { get; private set; }

        private Camera _camera;
        private Hotspot _hoveredHotspot;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            UpdateHover();
            UpdateClick();
        }

        private void UpdateHover()
        {
            Vector2 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Hotspot hotspot = null;
            if (hit.collider != null)
                hotspot = hit.collider.GetComponent<Hotspot>();

            if (hotspot != _hoveredHotspot)
            {
                if (_hoveredHotspot != null)
                    _hoveredHotspot.SetHighlight(false);

                _hoveredHotspot = (hotspot != null && hotspot.IsAvailable()) ? hotspot : null;

                if (_hoveredHotspot != null)
                {
                    _hoveredHotspot.SetHighlight(true);
                    CursorManager.Instance?.SetInteract();
                }
                else
                {
                    CursorManager.Instance?.SetDefault();
                }
            }
        }

        private void UpdateClick()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (_hoveredHotspot == null) return;

            HotspotAction action = _hoveredHotspot.GetAction();
            ExecuteAction(action);
        }

        private void ExecuteAction(HotspotAction action)
        {
            var state = GameManager.Instance.State;

            if (!string.IsNullOrEmpty(action.ItemToConsume))
                InventoryManager.Instance?.RemoveItem(action.ItemToConsume);

            if (!string.IsNullOrEmpty(action.FlagToSet))
                state.SetFlag(action.FlagToSet);

            if (!string.IsNullOrEmpty(action.ItemToGrant))
                InventoryManager.Instance?.AddItem(action.ItemToGrant);

            switch (action.Type)
            {
                case HotspotType.Examine:
                    NarrativeManager.Instance?.ShowText(action.ExamineText);
                    break;

                case HotspotType.PickUp:
                    NarrativeManager.Instance?.ShowText(action.ExamineText);
                    break;

                case HotspotType.Door:
                    SceneLoader.Instance?.LoadScene(action.TargetScene, action.IsNewSection);
                    break;

                case HotspotType.Terminal:
                case HotspotType.Narration:
                    if (!string.IsNullOrEmpty(action.NarrativeEntryId))
                        NarrativeManager.Instance?.PlayEntry(action.NarrativeEntryId);
                    else
                        NarrativeManager.Instance?.ShowText(action.ExamineText);
                    break;
            }
        }
    }
}
