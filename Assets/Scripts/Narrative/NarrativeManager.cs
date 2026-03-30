using System.Collections.Generic;
using UnityEngine;
using Signal.Core;

namespace Signal.Narrative
{
    public class NarrativeManager : MonoBehaviour
    {
        public static NarrativeManager Instance { get; private set; }

        [SerializeField] private NarrativeUI _narrativeUI;
        [SerializeField] private AudioSource _voiceSource;
        [SerializeField] private List<NarrativeEntry> _allEntries = new();

        private readonly Dictionary<string, NarrativeEntry> _entryLookup = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            foreach (var entry in _allEntries)
                _entryLookup[entry.EntryId] = entry;
        }

        public void ShowText(string text)
        {
            _narrativeUI.Show(text);
        }

        public void PlayEntry(string entryId)
        {
            if (!_entryLookup.TryGetValue(entryId, out var entry))
            {
                Debug.LogWarning($"Narrative entry not found: {entryId}");
                return;
            }

            var state = GameManager.Instance.State;
            bool useAlt = !string.IsNullOrEmpty(entry.AltConditionFlag) && state.HasFlag(entry.AltConditionFlag);

            string text = useAlt ? entry.AltText : entry.Text;
            AudioClip clip = useAlt ? entry.AltVoiceClip : entry.VoiceClip;

            _narrativeUI.Show(text);

            if (clip != null)
            {
                _voiceSource.Stop();
                _voiceSource.clip = clip;
                _voiceSource.Play();
            }

            if (!string.IsNullOrEmpty(entry.FlagToSet))
                state.SetFlag(entry.FlagToSet);
        }

        private void Update()
        {
            if (_narrativeUI.IsDisplaying && Input.GetMouseButtonDown(0))
                _narrativeUI.RequestSkip();
        }
    }
}
