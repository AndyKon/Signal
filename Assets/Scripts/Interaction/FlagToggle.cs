using UnityEngine;
using Signal.Core;

namespace Signal.Interaction
{
    public class FlagToggle : MonoBehaviour
    {
        [SerializeField] private string _flag;
        [SerializeField] private Mode _mode = Mode.ShowWhenSet;
        [SerializeField] private GameObject _altObject;

        public enum Mode
        {
            ShowWhenSet,
            HideWhenSet,
            SwapWhenSet
        }

        private void Start()
        {
            Apply();
        }

        private void Apply()
        {
            bool flagSet = GameManager.Instance != null && GameManager.Instance.State.HasFlag(_flag);

            switch (_mode)
            {
                case Mode.ShowWhenSet:
                    gameObject.SetActive(flagSet);
                    break;
                case Mode.HideWhenSet:
                    gameObject.SetActive(!flagSet);
                    break;
                case Mode.SwapWhenSet:
                    gameObject.SetActive(!flagSet);
                    if (_altObject != null)
                        _altObject.SetActive(flagSet);
                    break;
            }
        }
    }
}
