using System;
using UnityEngine;

namespace Signal.Interaction
{
    public enum HotspotType
    {
        Examine,
        PickUp,
        Door,
        Terminal,
        Narration
    }

    [Serializable]
    public class HotspotCondition
    {
        public string RequiredFlag = "";
        public string RequiredItem = "";
        public string BlockedByFlag = "";
    }

    [Serializable]
    public class HotspotAction
    {
        public HotspotType Type;
        [TextArea(2, 5)]
        public string ExamineText = "";
        public string ItemToGrant = "";
        public string ItemToConsume = "";
        public string FlagToSet = "";
        public string TargetScene = "";
        public bool IsNewSection;
        public string NarrativeEntryId = "";
    }
}
