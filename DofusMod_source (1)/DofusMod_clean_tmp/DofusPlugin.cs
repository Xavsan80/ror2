using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace DofusMod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency("com.bepis.r2api.prefab")]
    [BepInDependency("com.bepis.r2api.language")]
    [BepInDependency("com.bepis.r2api.sound")]
    [BepInDependency("com.bepis.r2api.loadout")]
    [BepInDependency("com.bepis.r2api.damagetype")]
    public class DofusPlugin : BaseUnityPlugin
    {
        public const string GUID    = "dofusmod.sacrier_xelor";
        public const string NAME    = "DofusMod";
        public const string VERSION = "1.0.0";

        public static DofusPlugin instance;
        public static ConfigEntry<bool> voiceLinesEnabled;

        private void Awake()
        {
            instance = this;

            voiceLinesEnabled = Config.Bind(
                "General", "EnableVoiceLines", true,
                "Toggle voice lines for both characters.");

            Characters.Sacrier.SacrierTokens.Register();
            Characters.Xelor.XelorTokens.Register();

            Characters.Sacrier.SacrierCharacter.Create();
            Characters.Xelor.XelorCharacter.Create();

            Log.Init(Logger);
            Log.Info("DofusMod loaded — Sacrier & Xelor ready!");
        }
    }

    internal static class Log
    {
        private static BepInEx.Logging.ManualLogSource _src;
        internal static void Init(BepInEx.Logging.ManualLogSource src) => _src = src;
        internal static void Info(object msg)    => _src.LogInfo(msg);
        internal static void Warning(object msg) => _src.LogWarning(msg);
        internal static void Error(object msg)   => _src.LogError(msg);
    }
}
