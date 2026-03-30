using UnityEngine;

namespace Signal.Narrative
{
    [CreateAssetMenu(fileName = "NewNarrative", menuName = "Signal/Narrative Entry")]
    public class NarrativeEntry : ScriptableObject
    {
        public string EntryId;

        [Header("Default Content")]
        [TextArea(3, 8)]
        public string Text;
        public AudioClip VoiceClip;

        [Header("Alternative Content (shown when AltConditionFlag is set)")]
        public string AltConditionFlag;
        [TextArea(3, 8)]
        public string AltText;
        public AudioClip AltVoiceClip;

        [Header("Effects")]
        public string FlagToSet;
    }
}
