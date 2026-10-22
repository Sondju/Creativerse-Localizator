using System;
using System.Collections.Generic;

namespace CreativerseLocalizator
{
    public static class TranslationStore
    {
        // Теперь это публичное статическое поле — доступно из любого места
        public static Dictionary<string, string> CustomTranslations { get; private set; } =
            new Dictionary<string, string>(StringComparer.Ordinal);

        public static void SetTranslations(Dictionary<string, string> newDict)
        {
            CustomTranslations = newDict ?? new Dictionary<string, string>(StringComparer.Ordinal);
        }

        public static void Clear()
        {
            CustomTranslations.Clear();
        }
    }
}