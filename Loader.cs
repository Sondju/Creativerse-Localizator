using I2.Loc;
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
        // Теперь целевая папка — Mods/Localization
        private static readonly string LocalizationDir = Path.Combine(MelonEnvironment.ModsDirectory, "Localization");

        public static void LoadCustomTranslations()
        {
            // Если папки Localization еще нет, создаем её для удобства пользователя
            if (!Directory.Exists(LocalizationDir))
            {
                Directory.CreateDirectory(LocalizationDir);
                MelonLogger.Warning($"[TranslationLoader] Папка не найдена. Создана пустая директория: {LocalizationDir}");
                return;
            }

            try
            {
                // Находим ВСЕ файлы с расширением .json в папке Mods/Localization и всех подпапках
                string[] jsonFiles = Directory.GetFiles(LocalizationDir, "*.json", SearchOption.AllDirectories);

                if (jsonFiles.Length == 0)
                {
                    MelonLogger.Warning($"[TranslationLoader] В папке {LocalizationDir} не найдено ни одного .json файла.");
                    return;
                }

                // Создаем один общий временный словарь для объединения всех файлов
                var combinedDict = new Dictionary<string, string>(StringComparer.Ordinal);
                int totalEntriesCount = 0;

                foreach (string filePath in jsonFiles)
                {
                    try
                    {
                        string json = File.ReadAllText(filePath, Encoding.UTF8);
                        var bundle = JsonConvert.DeserializeObject<LocalizationBundle>(json);

                        if (bundle?.Entries == null || bundle.Entries.Count == 0) continue;

                        int fileEntriesCount = 0;
                        foreach (var entry in bundle.Entries)
                        {
                            if (!string.IsNullOrEmpty(entry.Key) && !string.IsNullOrEmpty(entry.Translation))
                            {
                                // Проверка на дубликаты ключей между разными файлами
                                if (!combinedDict.ContainsKey(entry.Key))
                                {
                                    combinedDict[entry.Key] = entry.Translation;
                                    fileEntriesCount++;
                                }
                                else
                                {
                                    MelonLogger.Warning($"[TranslationLoader] Дубликат ключа '{entry.Key}' в файле: {Path.GetFileName(filePath)}. Пропущено.");
                                }
                            }
                        }

                        totalEntriesCount += fileEntriesCount;
                        MelonLogger.Msg($"[TranslationLoader] Из файла '{Path.GetFileName(filePath)}' успешно загружено строк: {fileEntriesCount}");
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"[TranslationLoader] Ошибка при чтении файла {Path.GetFileName(filePath)}: {ex.Message}");
                    }
                }

                // Записываем объединенный словарь в ваше публичное хранилище ОЗУ
                TranslationStore.SetTranslations(combinedDict);
                MelonLogger.Msg($"[TranslationLoader] ИТОГО загружено строк в кэш ОЗУ: {combinedDict.Count} (из {jsonFiles.Length} файлов)");

                // Связываем с движком игры I2.Loc
                LocalizationManager.OnLocalizeEvent += InjectTranslations;
                InjectTranslations();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[TranslationLoader] Глобальная ошибка при сканировании директории: {ex.Message}");
            }
        }

        private static void InjectTranslations()
        {
            // Если игра еще не инициализировала базы локализации, ждем
            if (LocalizationManager.Sources.Count == 0) return;

            // Получаем наш словарь из TranslationStore
            var myTranslations = TranslationStore.CustomTranslations;
            if (myTranslations == null || myTranslations.Count == 0) return;

            // Проходим по всем языковым базам игры
            foreach (LanguageSource source in LocalizationManager.Sources)
            {
                // Нам нужно определить, какой индекс в массиве занимает английский язык (обычно 0)
                int englishIndex = source.GetLanguageIndex("English");
                if (englishIndex == -1) englishIndex = 0; // На случай, если метод вернул ошибку, берем 0 по умолчанию

                foreach (TermData termData in source.mTerms)
                {
                    // Защита: проверяем, что массив языков существует и в нем есть нужный индекс
                    if (termData.Languages == null || termData.Languages.Length <= englishIndex) continue;

                    // Ищем ключ текущего игрового термина (например, "UI/Menu/Start") в нашем словаре
                    if (myTranslations.TryGetValue(termData.Term, out string ruText))
                    {
                        // Записываем русский перевод прямо в слот английского языка
                        termData.Languages[englishIndex] = ruText;

                        // На всякий случай проверяем тач-версию строк (для мобилок/геймпадов), 
                        // если игра использует Languages_Touch
                        if (termData.Languages_Touch != null && termData.Languages_Touch.Length > englishIndex)
                        {
                            termData.Languages_Touch[englishIndex] = ruText;
                        }
                    }
                }
            }

            MelonLogger.Msg("[TranslationLoader] Перевод успешно применен к I2.Loc!");
        }
    }
}