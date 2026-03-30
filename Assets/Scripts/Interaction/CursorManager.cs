using UnityEngine;

namespace Signal.Interaction
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        [SerializeField] private Texture2D _defaultCursor;
        [SerializeField] private Texture2D _interactCursor;
        [SerializeField] private Vector2 _hotspot = Vector2.zero;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetDefault() => Cursor.SetCursor(_defaultCursor, _hotspot, CursorMode.Auto);
        public void SetInteract() => Cursor.SetCursor(_interactCursor, _hotspot, CursorMode.Auto);
    }
}
