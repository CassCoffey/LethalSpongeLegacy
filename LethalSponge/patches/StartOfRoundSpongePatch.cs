using HarmonyLib;
using Scoops.service;
using UnityEngine;

namespace Scoops.patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundSpongePatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartOfRound_Start(ref StartOfRound __instance)
        {
            if (Config.fixCameraSettings.Value)
            {
                if (CameraService.Init())
                {
                    CameraService.ApplyCameraFixes();
                }
            }

            CameraService.DisablePosterization();
        }

        [HarmonyPatch("PassTimeToNextDay")]
        [HarmonyPostfix]
        private static void StartOfRound_PassTimeToNextDay(ref StartOfRound __instance)
        {
            if (Config.unloadUnused.Value)
            {
                Plugin.Log.LogMessage("Calling Resources.UnloadUnusedAssets().");
                Resources.UnloadUnusedAssets();
            }
        }
    }
}