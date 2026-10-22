using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CreativerseLocalizator
{
    public static class TranslationLoader
    {
        private static readonly string JsonPath = Path.Combine(MelonEnvironment.ModsDirectory, "localization_keys.json");

        public static void LoadCustomTranslations()
        {
            if (!File.Exists(JsonPath))
            {
                MelonLogger.Warning($"[TranslationLoader] Файл не найден: {JsonPath}. Переводы не загружены.");
                return;
            }

            try
            {
                string json = File.ReadAllText(JsonPath, Encoding.UTF8);
                var bundle = JsonConvert.DeserializeObject<LocalizationBundle>(json);

                var newDict = new Dictionary<string, string>(StringComparer.Ordinal);

                if (bundle?.Entries != null)
                {
                    foreach (var entry in bundle.Entries)
                    {
                        if (!string.IsNullOrEmpty(entry.Key) && !string.IsNullOrEmpty(entry.RuTranslation))
                        {
                            newDict[entry.Key] = entry.RuTranslation;
                        }
                    }
                }

                // Записываем в наше публичное хранилище ОЗУ
                TranslationStore.SetTranslations(newDict);
                MelonLogger.Msg($"[TranslationLoader] Успешно загружено переводов в кэш ОЗУ: {newDict.Count}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[TranslationLoader] Ошибка при загрузке JSON: {ex.Message}");
            }
        }
    }
}