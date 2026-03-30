using System;
using System.Collections.Generic;

namespace Signal.Core
{
    [Serializable]
    public class SaveData
    {
        public List<string> Flags = new();
        public List<int> PoweredSections = new();
        public string CurrentScene = "";
        public List<string> InventoryItems = new();
        public int TotalOptionalFlags;
    }
}
