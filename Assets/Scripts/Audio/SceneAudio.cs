using UnityEngine;

namespace Signal.Audio
{
    public class SceneAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip _ambienceClip;
        [SerializeField] private AudioClip _musicClip;

        private void Start()
        {
            if (AudioManager.Instance == null) return;

            if (_ambienceClip != null)
                AudioManager.Instance.PlayAmbience(_ambienceClip);

            if (_musicClip != null)
                AudioManager.Instance.PlayMusic(_musicClip);
        }
    }
}
