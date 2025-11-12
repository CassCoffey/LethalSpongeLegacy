using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Scoops.patches;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using System;

namespace Scoops;

public static class PluginInformation
{
    public const string PLUGIN_GUID = "LethalSpongeLegacy";
    public const string PLUGIN_NAME = "LethalSpongeLegacy";
    public const string PLUGIN_VERSION = "1.0.0";
}

[BepInPlugin(PluginInformation.PLUGIN_GUID, PluginInformation.PLUGIN_NAME, PluginInformation.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; set; }

    public static Config SpongeConfig { get; internal set; }

    public static AssetBundle SpongeAssets;

    public static ManualLogSource Log => Instance.Logger;

    private readonly Harmony _harmony = new(PluginInformation.PLUGIN_GUID);

    public Plugin()
    {
        Instance = this;
    }

    private void Awake()
    {
        Log.LogInfo("Loading LethalSpongeLegacy Version " + PluginInformation.PLUGIN_VERSION);

        var dllFolderPath = System.IO.Path.GetDirectoryName(Info.Location);
        var assetBundleFilePath = System.IO.Path.Combine(dllFolderPath, "spongeassets");
        SpongeAssets = AssetBundle.LoadFromFile(assetBundleFilePath);

        SpongeConfig = new(base.Config);

        Log.LogInfo($"Applying base patches...");
        ApplyPluginPatch();
        Log.LogInfo($"Base patches applied");

        AlterQualitySettings();
    }

    private void ApplyPluginPatch()
    {
        _harmony.PatchAll(typeof(StartOfRoundSpongePatch));

        if (Scoops.Config.unloadUnused.Value)
        {
            _harmony.PatchAll(typeof(MainMenuSpongePatch));
        }

        if (Scoops.Config.patchCameraScript.Value)
        {
            _harmony.PatchAll(typeof(ManualCameraRendererSpongePatch));
        }

        if (Scoops.Config.vSyncCount.Value != 1)
        {
            _harmony.PatchAll(typeof(IngamePlayerSettingsSpongePatch));
        }
    }

    private void AlterQualitySettings()
    {
        if (!Scoops.Config.qualityOverrides.Value) return;

        RenderPipelineSettings settings = ((HDRenderPipelineAsset)GraphicsSettings.currentRenderPipeline).currentPlatformRenderPipelineSettings;

        // if the settings are set to the highest value/default, don't override
        if (Scoops.Config.decalDrawDist.Value != 1000)
        {
            settings.decalSettings.drawDistance = Scoops.Config.decalDrawDist.Value;
        }
        if (Scoops.Config.decalAtlasSize.Value != 4096)
        {
            settings.decalSettings.atlasHeight = Scoops.Config.decalAtlasSize.Value;
            settings.decalSettings.atlasWidth = Scoops.Config.decalAtlasSize.Value;
        }

        if (Scoops.Config.reflectionAtlasSize.Value != "Resolution16384x8192")
        {
            settings.lightLoopSettings.reflectionProbeTexCacheSize = Enum.Parse<ReflectionProbeTextureCacheResolution>(Scoops.Config.reflectionAtlasSize.Value);
        }
        if (Scoops.Config.maxCubeReflectionProbes.Value != 48)
        {
            settings.lightLoopSettings.maxCubeReflectionOnScreen = Scoops.Config.maxCubeReflectionProbes.Value;
        }
        if (Scoops.Config.maxPlanarReflectionProbes.Value != 16)
        {
            settings.lightLoopSettings.maxPlanarReflectionOnScreen = Scoops.Config.maxPlanarReflectionProbes.Value;
        }

        if (Scoops.Config.shadowsMaxResolution.Value != 2048)
        {
            settings.hdShadowInitParams.maxPunctualShadowMapResolution = Scoops.Config.shadowsMaxResolution.Value;
            settings.hdShadowInitParams.maxDirectionalShadowMapResolution = Scoops.Config.shadowsMaxResolution.Value;
            settings.hdShadowInitParams.maxAreaShadowMapResolution = Scoops.Config.shadowsMaxResolution.Value;
        }
        if (Scoops.Config.shadowsAtlasSize.Value != 4096)
        {
            settings.hdShadowInitParams.punctualLightShadowAtlas.shadowAtlasResolution = Scoops.Config.shadowsAtlasSize.Value;
            settings.hdShadowInitParams.cachedPunctualLightShadowAtlas = Scoops.Config.shadowsAtlasSize.Value / 2; // Just make it half size for now
            settings.hdShadowInitParams.areaLightShadowAtlas.shadowAtlasResolution = Scoops.Config.shadowsAtlasSize.Value;
            settings.hdShadowInitParams.cachedAreaLightShadowAtlas = Scoops.Config.shadowsAtlasSize.Value / 2;
        }

        ((HDRenderPipelineAsset)GraphicsSettings.currentRenderPipeline).m_RenderPipelineSettings = settings;
        ((HDRenderPipelineAsset)GraphicsSettings.currentRenderPipeline).OnValidate();
    }
}
