using System.Collections;
using UnityEngine;

namespace Signal.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _ambienceSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private float _crossfadeDuration = 1.5f;

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

        public void PlayMusic(AudioClip clip)
        {
            if (_musicSource.clip == clip) return;
            StartCoroutine(CrossfadeSource(_musicSource, clip));
        }

        public void PlayAmbience(AudioClip clip)
        {
            if (_ambienceSource.clip == clip) return;
            StartCoroutine(CrossfadeSource(_ambienceSource, clip));
        }

        public void PlaySFX(AudioClip clip)
        {
            _sfxSource.PlayOneShot(clip);
        }

        public void StopMusic() => StartCoroutine(FadeOut(_musicSource));
        public void StopAmbience() => StartCoroutine(FadeOut(_ambienceSource));

        private IEnumerator CrossfadeSource(AudioSource source, AudioClip newClip)
        {
            float startVolume = source.volume;

            if (source.isPlaying)
            {
                yield return FadeOut(source);
            }

            source.clip = newClip;
            source.volume = 0f;
            source.Play();

            float elapsed = 0f;
            while (elapsed < _crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(0f, startVolume, elapsed / _crossfadeDuration);
                yield return null;
            }
            source.volume = startVolume;
        }

        private IEnumerator FadeOut(AudioSource source)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < _crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / _crossfadeDuration);
                yield return null;
            }

            source.Stop();
            source.volume = startVolume;
        }
    }
}
