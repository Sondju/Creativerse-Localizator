using System.Collections.Generic;

namespace CreativerseLocalizator
{
    public class LocalizationEntry
    {
        public string Key { get; set; }
        public string Original { get; set; }
        public string Translation { get; set; }
    }

    public class LocalizationBundle
    {
        public List<LocalizationEntry> Entries { get; set; } = new List<LocalizationEntry>();
    }
}