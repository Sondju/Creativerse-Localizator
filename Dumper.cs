using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using AccessTools = HarmonyLib.AccessTools;
using Formatting = Newtonsoft.Json.Formatting;

namespace CreativerseLocalizator
{
    public static class I2LocJsonDumper
    {
        private static readonly string OutputPath = Path.Combine(MelonEnvironment.ModsDirectory, "localization_dump.json");

        public static void ExportI2Localization()
        {
            var bundle = new LocalizationBundle();

            // Находим тип главного статического менеджера локализации I2.Loc
            var locMgrType = AccessTools.TypeByName("LocalizationManager");
            if (locMgrType == null)
            {
                MelonLogger.Error("[I2LocJsonDumper] Критическая ошибка! Не найден тип LocalizationManager.");
                return;
            }

            // Принудительно вызываем метод инициализации базы данных, если игра ещё не успела её прогрузить в ОЗУ!
            var initMethod = AccessTools.Method(locMgrType, "InitializeIfNeeded");
            if (initMethod != null)
            {
                try
                {
                    initMethod.Invoke(null, null);
                    MelonLogger.Msg("[I2LocJsonDumper] Вызвана рантайм-инициализация InitializeIfNeeded.");
                }
                catch (Exception e)
                {
                    MelonLogger.Warning($"[I2LocJsonDumper] Не удалось вызвать InitializeIfNeeded: {e.Message}");
                }
            }

            // === СЕРХИО, НАШ ГЛАВНЫЙ АРХИТЕКТУРНЫЙ ФИКС! ===
            // Поле Sources является СТАТИЧЕСКИМ, поэтому мы ищем его напрямую в типе класса!
            var sourcesField = AccessTools.Field(locMgrType, "Sources");
            if (sourcesField == null)
                sourcesField = AccessTools.Field(locMgrType, "mCurrentLanguage"); // Запасной поиск скрытых полей

            if (sourcesField == null)
            {
                MelonLogger.Error("[I2LocJsonDumper] Статическое поле 'Sources' не найдено в классе LocalizationManager.");
                return;
            }

            // Поскольку поле статическое, передаем null вместо экземпляра класса в GetValue!
            var sourcesObj = sourcesField.GetValue(null);

            if (sourcesObj == null)
            {
                MelonLogger.Warning("[I2LocJsonDumper] База данных 'Sources' в памяти пуста. Зайди в игровой мир или меню.");
                return;
            }

            if (!(sourcesObj is IList sourcesList))
            {
                MelonLogger.Error("[I2LocJsonDumper] Объект Sources не реализует интерфейс IList. Дамп невозможен.");
                return;
            }

            MelonLogger.Msg($"[I2LocJsonDumper] Обнаружено источников локализации в ОЗУ: {sourcesList.Count}. Начинаем глубокий экспорт строк...");

            // Обходим все прогруженные таблицы языковых ассетов игры
            foreach (var sourceObj in sourcesList)
            {
                if (sourceObj == null) continue;
                DumpLanguageSource(sourceObj, bundle);
            }

            // Упаковываем наш чистый bundle-контейнер в красивый структурированный JSON
            string json = JsonConvert.SerializeObject(bundle, Formatting.Indented);
            File.WriteAllText(OutputPath, json, Encoding.UTF8);

            MelonLogger.Msg($"[I2LocJsonDumper] Триумф! Выгрузка завершена! Всего экспортировано ключей: {bundle.Entries.Count}. Файл сохранен в:\n{OutputPath}");
        }

        private static void DumpLanguageSource(object sourceObj, LocalizationBundle bundle)
        {
            if (sourceObj == null || bundle == null) return;

            // Находим тип данных элементов списка терминов из I2.Loc
            var termDataType = AccessTools.TypeByName("TermData");
            if (termDataType == null)
            {
                MelonLogger.Error("[I2LocJsonDumper] Не найден тип данных TermData.");
                return;
            }

            // === СЕРХИО, УЛЬТИМАТИВНЫЙ ФИКС СЛЕПОГО ПОИСКА! ===
            // Вместо хардкода "m_Terms" или "Terms", мы сканируем ВСЕ поля класса и ищем коллекцию List<TermData>!
            FieldInfo termsField = null;
            var allFields = AccessTools.GetDeclaredFields(sourceObj.GetType());

            foreach (var field in allFields)
            {
                // Проверяем, является ли поле дженерик-листом (List<T>)
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // Проверяем, что внутри листа лежат именно объекты TermData
                    var genericArgument = field.FieldType.GetGenericArguments()[0];
                    if (genericArgument == termDataType)
                    {
                        termsField = field;
                        MelonLogger.Msg($"[I2LocJsonDumper] Успешно обнаружена скрытая коллекция терминов по типу: [{field.Name}]");
                        break;
                    }
                }
            }

            // Резервный хардкод-фоллбэк на случай, если типы упакованы иначе
            if (termsField == null) termsField = AccessTools.Field(sourceObj.GetType(), "mTerms");
            if (termsField == null) termsField = AccessTools.Field(sourceObj.GetType(), "m_Terms");

            if (termsField == null)
            {
                MelonLogger.Error("[I2LocJsonDumper] Не удалось обнаружить поле списка терминов внутри LanguageSource.");
                return;
            }

            // Извлекаем коллекцию из нашего LanguageSource объекта
            var termsObj = termsField.GetValue(sourceObj);
            if (!(termsObj is IList termsList) || termsList.Count == 0)
            {
                MelonLogger.Warning($"[I2LocJsonDumper] Список терминов в поле {termsField.Name} пришел пустым или null.");
                return;
            }

            // Подтягиваем метаданные полей самого TermData для сборки строк в JSON
            var termField = AccessTools.Field(termDataType, "Term");
            var languagesField = AccessTools.Field(termDataType, "Languages");

            if (termField == null || languagesField == null)
            {
                MelonLogger.Error("[I2LocJsonDumper] Ошибка! Не найдены поля 'Term' или 'Languages' внутри класса TermData.");
                return;
            }
        }
    }
}