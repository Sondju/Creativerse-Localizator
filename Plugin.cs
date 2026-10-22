using HarmonyLib;
using MelonLoader;
using System;
using UnityEngine;

[assembly: MelonInfo(typeof(CreativerseLocalizator.ModCore), "Localizator API", "1.0.0", "Sergio")]
[assembly: MelonGame]

namespace CreativerseLocalizator
{
    public class ModCore : MelonMod
    {
        public override void OnInitializeMelon()
        {
            try
            {
                // Принудительно пинаем загрузчик JSON
                TranslationLoader.LoadCustomTranslations();

                MelonLogger.Msg("Ультимативный C#-пылесос строк запущен! Нажми F10 в игре для дампа памяти.");
            }
            catch (Exception ex)
            {
                // Если Мелон споткнулся о скрытый конфликт библиотек — мы выведем его на чистую воду!
                MelonLogger.Error($"[ModCore] КРИТИЧЕСКИЙ СБОЙ ИНИЦИАЛИЗАЦИИ МОДА: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                I2LocJsonDumper.ExportI2Localization();
            }
        }

        [HarmonyPatch(typeof(I2.Loc.LocalizationManager), "GetTermTranslation", new[] { typeof(string) })]
        public class LocalizationGetTermTranslationPatch
        {
            [HarmonyPostfix]
            public static void Postfix(string Term, ref string __result)
            {
                if (TranslationStore.CustomTranslations.TryGetValue(Term, out var custom))
                {
                    __result = custom;
                }
            }
        }
    }
}