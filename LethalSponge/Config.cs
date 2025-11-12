using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace Scoops
{
    public class Config
    {
        public static ConfigEntry<bool> unloadUnused;

        public static ConfigEntry<bool> fixCameraSettings;
        public static ConfigEntry<bool> applyShipCameraQualityOverrides;
        public static ConfigEntry<bool> applySecurityCameraQualityOverrides;
        public static ConfigEntry<bool> applyMapCameraQualityOverrides;
        public static ConfigEntry<bool> cameraRenderTransparent;
        public static ConfigEntry<bool> patchCameraScript;
        public static ConfigEntry<float> securityCameraCullDistance;
        public static ConfigEntry<int> mapCameraFramerate;
        public static ConfigEntry<int> securityCameraFramerate;
        public static ConfigEntry<int> shipCameraFramerate;

        public static ConfigEntry<int> vSyncCount;

        public static ConfigEntry<bool> qualityOverrides;
        public static ConfigEntry<int> decalDrawDist;
        public static ConfigEntry<int> decalAtlasSize;
        public static ConfigEntry<string> reflectionAtlasSize;
        public static ConfigEntry<int> maxCubeReflectionProbes;
        public static ConfigEntry<int> maxPlanarReflectionProbes;
        public static ConfigEntry<int> shadowsMaxResolution;
        public static ConfigEntry<int> shadowsAtlasSize;

        public Config(ConfigFile cfg)
        {
            cfg.SaveOnConfigSet = false;

            // Cleanup
            unloadUnused = cfg.Bind(
                    "Cleanup",
                    "unloadUnused",
                    true,
                    "Should Sponge call UnloadUnusedAssets each day?"
            );

            // Cameras
            fixCameraSettings = cfg.Bind(
                    "Cameras",
                    "fixCameraSettings",
                    true,
                    "Should Sponge change the settings for the ship cameras and radar cam to improve performance?"
            );
            applyShipCameraQualityOverrides = cfg.Bind(
                    "Cameras",
                    "applyShipCameraQualityOverrides",
                    true,
                    "Should Sponge disable extra HDRP rendering features on the Ship camera? (Requires fixCameraSettings = true)"
            );
            applySecurityCameraQualityOverrides = cfg.Bind(
                    "Cameras",
                    "applySecurityCameraQualityOverrides",
                    true,
                    "Should Sponge disable extra HDRP rendering features on the Security camera? (Requires fixCameraSettings = true)"
            );
            applyMapCameraQualityOverrides = cfg.Bind(
                    "Cameras",
                    "applyMapCameraQualityOverrides",
                    true,
                    "Should Sponge disable extra HDRP rendering features on the Map camera? (Requires fixCameraSettings = true)"
            );
            cameraRenderTransparent = cfg.Bind(
                    "Cameras",
                    "cameraRenderTransparent",
                    true,
                    "Should the Ship and Security camera render transparent objects? (Requires applyShipCameraQualityOverrides or applySecurityCameraQualityOverrides)"
            );
            patchCameraScript = cfg.Bind(
                    "Cameras",
                    "patchCameraScript",
                    true,
                    "Should Sponge replace the base Lethal Company ManualCameraRenderer.MeetsCameraEnabledConditions function with one that more reliably disables ship cameras when they're not in view?"
            );
            securityCameraCullDistance = cfg.Bind(
                    "Cameras",
                    "securityCameraCullDistance",
                    20f,
                    new ConfigDescription("What should the culling distance be for the ship security camera? You might want to increase this if you're using a mod to re-add planets in orbit. (LC default is 150)", new AcceptableValueRange<float>(15, 150))
            );
            securityCameraFramerate = cfg.Bind(
                    "Cameras",
                    "securityCameraFramerate",
                    15,
                    "What framerate should the exterior cam run at? 0 = not limited. (Requires fixCameraSettings = true)"
            );
            shipCameraFramerate = cfg.Bind(
                    "Cameras",
                    "shipCameraFramerate",
                    15,
                    "What framerate should the interior cam run at? 0 = not limited. (Requires fixCameraSettings = true)"
            );
            mapCameraFramerate = cfg.Bind(
                    "Cameras",
                    "mapCameraFramerate",
                    20,
                    "What framerate should the radar map camera run at? 0 = not limited. (Requires fixCameraSettings = true)"
            );

            // Rendering
            vSyncCount = cfg.Bind(
                "Rendering",
                "vSyncCount",
                1,
                "When the option \"Use monitor (V-Sync)\" is selected in Options, what VSyncCount should be used? (LC default is 1)"
            );

            // Graphics Quality
            qualityOverrides = cfg.Bind(
                "Graphics Quality",
                "qualityOverrides",
                true,
                "Should Sponge change the default quality settings? This must be on for any of the other Graphics Quality settings to take effect."
            );
            decalDrawDist = cfg.Bind(
                "Graphics Quality",
                "decalDrawDist",
                100,
                new ConfigDescription("What should the maximum distance be for drawing decals like blood splatters? (LC default is 1000)", new AcceptableValueRange<int>(50, 1000))
            );
            decalAtlasSize = cfg.Bind(
                "Graphics Quality",
                "decalAtlasSize",
                2048,
                new ConfigDescription("What should the texture size be for the the Decal Atlas? (squared) (LC default is 4096)", new AcceptableValueList<int>(2048, 4096))
            );
            reflectionAtlasSize = cfg.Bind(
                "Graphics Quality",
                "reflectionAtlasSize",
                "Resolution1024x1024",
                new ConfigDescription("What should the texture size be for the the Reflection Atlas? (LC default is 16384x8192)", new AcceptableValueList<string>("Resolution512x512", "Resolution1024x512", "Resolution1024x1024", "Resolution2048x1024", "Resolution2048x2048", "Resolution4096x2048", "Resolution16384x8192"))
            );
            maxCubeReflectionProbes = cfg.Bind(
                "Graphics Quality",
                "maxCubeReflectionProbes",
                12,
                new ConfigDescription("How many Cube Reflection Probes should be able to be on screen at once? (LC default is 48)", new AcceptableValueRange<int>(6, 48))
            );
            maxPlanarReflectionProbes = cfg.Bind(
                "Graphics Quality",
                "maxPlanarReflectionProbes",
                8,
                new ConfigDescription("How many Cube Reflection Probes should be able to be on screen at once? (LC default is 16)", new AcceptableValueRange<int>(4, 16))
            );
            shadowsMaxResolution = cfg.Bind(
                "Graphics Quality",
                "shadowsMaxResolution",
                256,
                new ConfigDescription("What should the maximum resolution be for Shadow Maps? (LC default is 2048)", new AcceptableValueList<int>(64, 128, 256, 512, 1024, 2048))
            );
            shadowsAtlasSize = cfg.Bind(
                "Graphics Quality",
                "shadowsAtlasSize",
                2048,
                new ConfigDescription("What should the resolution be for the Shadow Map Atlas? (LC default is 4096)", new AcceptableValueList<int>(1024, 2048, 4096))
            );

            ClearOrphanedEntries(cfg);
            cfg.Save();
            cfg.SaveOnConfigSet = true;
        }

        // Thanks modding wiki
        static void ClearOrphanedEntries(ConfigFile cfg)
        {
            // Find the private property `OrphanedEntries` from the type `ConfigFile` //
            PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
            // And get the value of that property from our ConfigFile instance //
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
            // And finally, clear the `OrphanedEntries` dictionary //
            orphanedEntries.Clear();
        }
    }
}
