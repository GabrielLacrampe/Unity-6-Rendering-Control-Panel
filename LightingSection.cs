using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace RenderingControlPanel
{
    public class LightingSection : EditorSection
    {
        public override void Draw(GUIStyle boldLabel, bool showFocusedSettings)
        {
            DrawLighting_Section();
        }

        bool showLighting_Section = false;
        void DrawLighting_Section()
        {
            EditorGUILayout.BeginVertical("box");
            showLighting_Section = EditorGUILayout.Foldout(showLighting_Section, "Lighting", true);
            EditorGUILayout.EndVertical();

            if (showLighting_Section)
            {
                EditorGUI.indentLevel++;

                DrawScene_Window();
                DrawEnvironment_Window();

                EditorGUI.indentLevel--;
            }
        }

        #region Environment
        bool showEnvironment_Window = false;
        void DrawEnvironment_Window()
        {
            EditorGUILayout.BeginVertical("box");
            showEnvironment_Window = EditorGUILayout.Foldout(showEnvironment_Window, "Environment", true);
            EditorGUILayout.EndVertical();
            if (showEnvironment_Window)
            {
                EditorGUI.indentLevel++;

                DrawEnvironment_Menu();
                DrawOtherSettings_Menu();

                EditorGUI.indentLevel--;
            }
        }

        bool showEnvironment_Menu = false;
        void DrawEnvironment_Menu()
        {
            EditorGUILayout.BeginVertical("box");
            showEnvironment_Menu = EditorGUILayout.Foldout(showEnvironment_Menu, "Environment", true);
            EditorGUILayout.EndVertical();
            if (showEnvironment_Menu)
            {
                EditorGUI.indentLevel++;

                RenderSettings.skybox = (Material)EditorGUILayout.ObjectField("Skybox Material", RenderSettings.skybox, typeof(Material), false);
                RenderSettings.sun = (Light)EditorGUILayout.ObjectField("Sun Light", RenderSettings.sun, typeof(Light), true);

                EditorGUILayout.Space(5);
                RenderSettings.subtractiveShadowColor = EditorGUILayout.ColorField("Realtime Shadow Color", RenderSettings.subtractiveShadowColor);

                // Environment Lighting
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Environment Lighting", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                AmbientSource ambientSource = AmbientSource.Skybox;
                switch (RenderSettings.ambientMode)
                {
                    case AmbientMode.Skybox: ambientSource = AmbientSource.Skybox; break;
                    case AmbientMode.Flat: ambientSource = AmbientSource.Color; break;
                    case AmbientMode.Trilight: ambientSource = AmbientSource.Gradient; break;
                }
                ambientSource = (AmbientSource)EditorGUILayout.EnumPopup("Source", ambientSource);

                // Mapear selección al AmbientMode de Unity
                switch (ambientSource)
                {
                    case AmbientSource.Skybox: RenderSettings.ambientMode = AmbientMode.Skybox; break;
                    case AmbientSource.Color: RenderSettings.ambientMode = AmbientMode.Flat; break;
                    case AmbientSource.Gradient: RenderSettings.ambientMode = AmbientMode.Trilight; break;
                }

                switch (ambientSource)
                {
                    case AmbientSource.Skybox:
                        EditorGUILayout.HelpBox("La iluminación ambiental se toma del Skybox asignado.", MessageType.Info);
                        break;
                    case AmbientSource.Color:
                        RenderSettings.ambientLight = EditorGUILayout.ColorField(new GUIContent("Ambient Color", "Color HDR"), RenderSettings.ambientLight, true, true, true);
                        break;
                    case AmbientSource.Gradient:
                        RenderSettings.ambientSkyColor = EditorGUILayout.ColorField(new GUIContent("Sky Color", "Color HDR"), RenderSettings.ambientSkyColor, true, false, true);
                        RenderSettings.ambientEquatorColor = EditorGUILayout.ColorField(new GUIContent("Equator Color", "Color HDR"), RenderSettings.ambientEquatorColor, true, false, true);
                        RenderSettings.ambientGroundColor = EditorGUILayout.ColorField(new GUIContent("Ground Color", "Color HDR"), RenderSettings.ambientGroundColor, true, false, true);
                        break;
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Environment Reflections", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                RenderSettings.defaultReflectionMode = (DefaultReflectionMode)EditorGUILayout.EnumPopup("Source", RenderSettings.defaultReflectionMode);
                if (RenderSettings.defaultReflectionMode == DefaultReflectionMode.Custom)
                {
                    RenderSettings.customReflectionTexture = (Cubemap)EditorGUILayout.ObjectField("Custom Reflection", RenderSettings.customReflectionTexture, typeof(Cubemap), false);
                }

                // Enum para resolución
                ReflectionResolution currentRes = (ReflectionResolution)Mathf.Clamp(RenderSettings.defaultReflectionResolution, 16, 2048);
                currentRes = (ReflectionResolution)EditorGUILayout.EnumPopup("Resolution", currentRes);
                RenderSettings.defaultReflectionResolution = (int)currentRes;

                EditorGUILayout.LabelField("Compression");
                EditorGUILayout.HelpBox("Esta opción solo puede modificarse desde:\nWindow > Rendering > Lighting > Environment > Reflections > Compression", MessageType.Info);

                RenderSettings.reflectionIntensity = EditorGUILayout.Slider("Intensity Multiplier", RenderSettings.reflectionIntensity, 0f, 1f);
                RenderSettings.reflectionBounces = EditorGUILayout.IntSlider("Bounces", RenderSettings.reflectionBounces, 0, 4);

                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
            }
        }

        bool showOtherSettings_Menu = false;
        void DrawOtherSettings_Menu()
        {
            EditorGUILayout.BeginVertical("box");
            showOtherSettings_Menu = EditorGUILayout.Foldout(showOtherSettings_Menu, "Other Settings", true);
            EditorGUILayout.EndVertical();
            if (showOtherSettings_Menu)
            {
                EditorGUI.indentLevel++;

                RenderSettings.fog = EditorGUILayout.Toggle("Enable Fog", RenderSettings.fog);
                if (RenderSettings.fog)
                {
                    EditorGUI.indentLevel++;

                    RenderSettings.fogColor = EditorGUILayout.ColorField("Color", RenderSettings.fogColor);
                    RenderSettings.fogMode = (FogMode)EditorGUILayout.EnumPopup("Mode", RenderSettings.fogMode);
                    RenderSettings.fogStartDistance = EditorGUILayout.FloatField("Start", RenderSettings.fogStartDistance);
                    RenderSettings.fogEndDistance = EditorGUILayout.FloatField("End", RenderSettings.fogEndDistance);

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(5);

                // Halo
                EditorGUILayout.LabelField("Halo Texture");
                EditorGUILayout.HelpBox("Esta opción solo puede modificarse desde:\nWindow > Rendering > Lighting > Environment > Other Settings", MessageType.Info);
                RenderSettings.haloStrength = EditorGUILayout.Slider("Halo Strength", RenderSettings.haloStrength, 0f, 1f);

                // Flare
                RenderSettings.flareFadeSpeed = EditorGUILayout.FloatField("Flare Fade Speed", RenderSettings.flareFadeSpeed);
                RenderSettings.flareStrength = EditorGUILayout.Slider("Flare Strength", RenderSettings.flareStrength, 0f, 1f);

                // Spot Cookie
                EditorGUILayout.LabelField("Spot Cookiee");
                EditorGUILayout.HelpBox("Esta opción solo puede modificarse desde:\nWindow > Rendering > Lighting > Environment > Other Settings", MessageType.Info);

                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Scene
        bool showScene_Window = false;
        void DrawScene_Window()
        {
            var _lightingSettings = Lightmapping.lightingSettings;
            EditorGUILayout.BeginVertical("box");
            showScene_Window = EditorGUILayout.Foldout(showScene_Window, "Scene", true);
            EditorGUILayout.EndVertical();
            if (showScene_Window)
            {
                EditorGUI.indentLevel++;

                DrawLightingSettings_Menu();
                DrawRealtimeLighting_Menu(_lightingSettings);
                DrawMixedLighting_Menu(_lightingSettings);
                DrawLightmappingSettings_Menu(_lightingSettings);

                EditorGUI.indentLevel--;
            }
        }

        bool showLightingSettings_Menu = false;
        void DrawLightingSettings_Menu()
        {
            EditorGUILayout.BeginVertical("box");
            showLightingSettings_Menu = EditorGUILayout.Foldout(showLightingSettings_Menu, "Lighting Settings", true);
            EditorGUILayout.EndVertical();
            if (showLightingSettings_Menu)
            {
                EditorGUI.indentLevel++;

                //TODO: Lighting Setting Asset ObjectField (new/clone)
                Lightmapping.lightingSettings = (LightingSettings)EditorGUILayout.ObjectField(
                    "Lighting Settings Asset", Lightmapping.lightingSettings, typeof(LightingSettings), false);

                EditorGUI.indentLevel--;
            }
        }

        bool showRealtimeLighting_Menu = false;
        void DrawRealtimeLighting_Menu(LightingSettings _lightingSettings)
        {
            EditorGUILayout.BeginVertical("box");
            showRealtimeLighting_Menu = EditorGUILayout.Foldout(showRealtimeLighting_Menu, "Realtime Lighting", true);
            EditorGUILayout.EndVertical();
            if (showRealtimeLighting_Menu)
            {
                EditorGUI.indentLevel++;

                _lightingSettings.realtimeGI = EditorGUILayout.Toggle("Realtime Global Illumination", _lightingSettings.realtimeGI);
                _lightingSettings.realtimeEnvironmentLighting = EditorGUILayout.Toggle("Realtime Environment Lighting", _lightingSettings.realtimeEnvironmentLighting);
                _lightingSettings.indirectResolution = EditorGUILayout.FloatField("Indirect Resolution", _lightingSettings.indirectResolution);

                EditorGUI.indentLevel--;
            }
        }

        bool showMixedLighting_Menu = false;
        void DrawMixedLighting_Menu(LightingSettings _lightingSettings)
        {
            EditorGUILayout.BeginVertical("box");
            showMixedLighting_Menu = EditorGUILayout.Foldout(showMixedLighting_Menu, "Mixed Lighting", true);
            EditorGUILayout.EndVertical();
            if (showMixedLighting_Menu)
            {
                EditorGUI.indentLevel++;

                _lightingSettings.bakedGI = EditorGUILayout.Toggle("Baked Global Illumination", _lightingSettings.bakedGI);
                _lightingSettings.mixedBakeMode = (MixedLightingMode)EditorGUILayout.EnumPopup("Mixed Bake Mode", _lightingSettings.mixedBakeMode);

                EditorGUI.indentLevel--;
            }
        }
        bool showLightmappingSettings_Menu = false;

        private static readonly int[] SampleOptions = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
        LightmapParameters selectedLightmapParameters;
        void DrawLightmappingSettings_Menu(LightingSettings _lightingSettings)
        {
            EditorGUILayout.BeginVertical("box");
            showLightmappingSettings_Menu = EditorGUILayout.Foldout(showLightmappingSettings_Menu, "Lightmapping Settings", true);
            EditorGUILayout.EndVertical();
            if (showLightmappingSettings_Menu)
            {
                EditorGUI.indentLevel++;

                _lightingSettings.lightmapper = (LightingSettings.Lightmapper)EditorGUILayout.EnumPopup("Lightmapper", _lightingSettings.lightmapper);
                EditorGUI.indentLevel++;

                _lightingSettings.environmentImportanceSampling = EditorGUILayout.Toggle("Importance Sampling", _lightingSettings.environmentImportanceSampling);

                // TODO: se requiere transformar los samples count en un enum
                int directIndex = Mathf.Clamp(System.Array.IndexOf(SampleOptions, _lightingSettings.directSampleCount), 0, 10);
                directIndex = EditorGUILayout.IntSlider($"Direct Sample ({SampleOptions[directIndex]})", directIndex, 0, 10);
                _lightingSettings.directSampleCount = SampleOptions[directIndex];

                int indirectIndex = Mathf.Clamp(System.Array.IndexOf(SampleOptions, _lightingSettings.indirectSampleCount), 0, 13);
                indirectIndex = EditorGUILayout.IntSlider($"Indirect Sample ({SampleOptions[indirectIndex]})", indirectIndex, 0, 13);
                _lightingSettings.indirectSampleCount = SampleOptions[indirectIndex];

                int envIndex = Mathf.Clamp(System.Array.IndexOf(SampleOptions, _lightingSettings.environmentSampleCount), 0, 11);
                envIndex = EditorGUILayout.IntSlider($"Environment Sample ({SampleOptions[envIndex]})", envIndex, 0, 11);
                _lightingSettings.environmentSampleCount = SampleOptions[envIndex];

                _lightingSettings.lightProbeSampleCountMultiplier = EditorGUILayout.FloatField("Light Probe Sample Multiplier", _lightingSettings.lightProbeSampleCountMultiplier);
                _lightingSettings.maxBounces = EditorGUILayout.IntField("Max Bounces", _lightingSettings.maxBounces);

                _lightingSettings.filteringMode = (LightingSettings.FilterMode)EditorGUILayout.EnumPopup("Filtering", _lightingSettings.filteringMode);

                EditorGUI.indentLevel++;

                _lightingSettings.denoiserTypeDirect = (LightingSettings.DenoiserType)EditorGUILayout.EnumPopup("Direct Denoiser", _lightingSettings.denoiserTypeDirect);
                _lightingSettings.filterTypeDirect = (LightingSettings.FilterType)EditorGUILayout.EnumPopup("Direct Filter", _lightingSettings.filterTypeDirect);

                EditorGUI.indentLevel++;
                _lightingSettings.filteringGaussianRadiusDirect = EditorGUILayout.Slider("Radius", _lightingSettings.filteringGaussianRadiusDirect, 0f, 5f);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space(5);
                _lightingSettings.denoiserTypeIndirect = (LightingSettings.DenoiserType)EditorGUILayout.EnumPopup("Indirect Denoiser", _lightingSettings.denoiserTypeIndirect);
                _lightingSettings.filterTypeIndirect = (LightingSettings.FilterType)EditorGUILayout.EnumPopup("Indirect Filter", _lightingSettings.filterTypeIndirect);

                EditorGUI.indentLevel++;
                _lightingSettings.filteringGaussianRadiusIndirect = EditorGUILayout.Slider("Radius", _lightingSettings.filteringGaussianRadiusIndirect, 0f, 5f);
                EditorGUI.indentLevel--;

                EditorGUILayout.Space(5);
                // TODO: la sección AO se muestra en tono gris y no se puede usar si el bool ao es false, si es true además de estas opciones se muestran otras, max distance, indirect contribution y direct contribution
                GUI.enabled = _lightingSettings.ao;

                _lightingSettings.denoiserTypeAO = (LightingSettings.DenoiserType)EditorGUILayout.EnumPopup("Ambient Occlusion Denoiser", _lightingSettings.denoiserTypeAO);
                _lightingSettings.filterTypeAO = (LightingSettings.FilterType)EditorGUILayout.EnumPopup("Ambient Occlusion Filter", _lightingSettings.filterTypeAO);

                EditorGUI.indentLevel++;
                _lightingSettings.filteringGaussianRadiusAO = EditorGUILayout.Slider("Radius", _lightingSettings.filteringGaussianRadiusAO, 0f, 5f);
                GUI.enabled = true;
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;

                EditorGUILayout.Space(5);
                _lightingSettings.lightmapResolution = EditorGUILayout.FloatField("Lightmap Resolution", _lightingSettings.lightmapResolution);
                _lightingSettings.lightmapPadding = EditorGUILayout.IntField("Lightmap Padding", _lightingSettings.lightmapPadding);

                // TODO: lightmap max size es un enum
                int[] sizeOptions = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
                int currentIndex = Mathf.Clamp(System.Array.IndexOf(sizeOptions, _lightingSettings.lightmapMaxSize), 0, sizeOptions.Length - 1);
                currentIndex = EditorGUILayout.Popup("Max Lightmap Size", currentIndex, sizeOptions.Select(s => s.ToString()).ToArray());
                _lightingSettings.lightmapMaxSize = sizeOptions[currentIndex];

                //_lightingSettings.lightmapMaxSize = EditorGUILayout.IntField("Max Lightmap Size", _lightingSettings.lightmapMaxSize);

                // TODO: falta Fixed Lightmap Size
                // TODO: falta Use Mipmap Limits

                EditorGUILayout.Space(5);
                _lightingSettings.lightmapCompression = (LightmapCompression)EditorGUILayout.EnumPopup("Lightmap Compression", _lightingSettings.lightmapCompression);
                _lightingSettings.ao = EditorGUILayout.Toggle("Ambient Occlusion", _lightingSettings.ao);
                _lightingSettings.directionalityMode = (LightmapsMode)EditorGUILayout.EnumPopup("Directionality Mode", _lightingSettings.directionalityMode);
                _lightingSettings.albedoBoost = EditorGUILayout.Slider("Albedo Boost", _lightingSettings.albedoBoost, 0f, 10f);
                _lightingSettings.indirectScale = EditorGUILayout.Slider("Indirect Scale", _lightingSettings.indirectScale, 0f, 5f);

                // TODO: lightmap parameters no selecciona el asset en uso habiendo uno            
                selectedLightmapParameters = (LightmapParameters)EditorGUILayout.ObjectField("Lightmap Parameters", selectedLightmapParameters, typeof(LightmapParameters), false);

                // idea SerializedObject qualitySettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);

                EditorGUI.indentLevel--;
            }
        }
        #endregion

    }
}
