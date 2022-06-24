using flanne.UI;
using static DarkerNights.DarkerNights;
using HarmonyLib;
using flanne;
using UnityEngine;
using System.Collections.Generic;

namespace DarkerNights
{
    class DifficultyController_Patch
    {

        private static List<DifficultyModifier> DarkerModList = new List<DifficultyModifier>{
            ScriptableObject.CreateInstance<VisionModifier>().Init(true, "Shortsighted", "Years of playing Videogames ruined your eyesight."), //darkness 16
        };

        [HarmonyPatch(typeof(DifficultyController), "Init")]
        [HarmonyPrefix]
        public static bool InitPrefix(ref DifficultyController __instance, ref int maxDiff)
        {
            //get DifficultyModifiers
            DifficultyModList[] modLists = Resources.FindObjectsOfTypeAll<DifficultyModList>();
            foreach (DifficultyModList modList in modLists)
            {
                Log.LogInfo("Comparing ModList names " + modList.name + " " + DarkerNights.ConfigDifficultyModListName);
                if (modList.name == DarkerNights.ConfigDifficultyModListName.Value)
                {
                    Log.LogInfo("Patching darkness levels...");
                    //create new bigger array
                    DifficultyModifier[] patchedMods = new DifficultyModifier[modList.mods.Length + DarkerModList.Count];
                    //include base modifiers
                    modList.mods.CopyTo(patchedMods, 0);
                    //add own modifiers
                    for (int i = 0; i < (patchedMods.Length - modList.mods.Length); i++)
                    {
                        Log.LogInfo("Placing mod in slot " + (modList.mods.Length + i) + "/" + (patchedMods.Length - 1));
                        patchedMods[modList.mods.Length + i] = DarkerModList[i];
                    }
                    //overwrite original modlist
                    modList.mods = patchedMods;
                    //adjust max Difficulty
                    maxDiff = modList.mods.Length - 1;
                    Log.LogInfo("Patched successfully.");
                    break;
                }
                else
                {
                    Log.LogInfo("Couldn't find ModList");
                }
            }
            return true;
        }
    }
}