using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.IO;
using System;

namespace DarkerNights
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class DarkerNights : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        public static ConfigEntry<string> ConfigDifficultyModListName;

        private void Awake()
        {
            // Plugin startup logic
            //Setup config
            ConfigDifficultyModListName = Config.Bind<string>("General", "DifficultyModList Name", "DefaultDifficultyLevels", "Changes the DifficultyModList the Darkness levels are appended to. DO NOT CHANGE unless you know what you are doing.");

            Log = base.Logger;
            //Apply method patches through Harmony
            try
            {
                Harmony.CreateAndPatchAll(typeof(DifficultyController_Patch));
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
                Log.LogError($"{PluginInfo.PLUGIN_GUID} failed to patch methods.");
            }

            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        //expose DifficultyModList Name for mods that have their own custom darkness levels and want to provide compatibility
        public string getDifficultyModListName()
        {
            return ConfigDifficultyModListName.Value;
        }
        public void setDifficultyModListName(string name)
        {
            ConfigDifficultyModListName.Value = name;
        }


        private void OnDestroy()
        {
            Harmony.UnpatchAll();
        }
    }
}
