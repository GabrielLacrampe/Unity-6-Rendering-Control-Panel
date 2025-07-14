using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace RenderingControlPanel
{
    public class QualitySettingsSection : EditorSection
    {
        public override void Draw(GUIStyle boldLabel, bool showFocusedSettings)
        {
            DrawQualitySettings_Section(boldLabel);
        }

        #region Quality
        bool showQualityWindow = false;
        void DrawQualitySettings_Section(GUIStyle boldLabel)
        {
            EditorGUILayout.BeginVertical("box");
            showQualityWindow = EditorGUILayout.Foldout(showQualityWindow, "Quality", true);
            EditorGUILayout.EndVertical();

            string[] qualityLevels = QualitySettings.names;
            int currentQuality = QualitySettings.GetQualityLevel();

            SerializedObject qualitySettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/QualitySettings.asset")[0]);

            if (showQualityWindow)
            {
                EditorGUI.indentLevel++;

                int selectedQuality = EditorGUILayout.Popup("Levels", currentQuality, qualityLevels);
                if (selectedQuality != currentQuality) QualitySettings.SetQualityLevel(selectedQuality, true);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                EditorGUILayout.LabelField("Current Build Target: " + EditorUserBuildSettings.activeBuildTarget);

                EditorGUILayout.Space(5);
                DrawRendering_Window(boldLabel);
                EditorGUILayout.Space(5);
                DrawTexture_Window(boldLabel);
                EditorGUILayout.Space(5);
                DrawParticles_Window(boldLabel);
                EditorGUILayout.Space(5);
                DrawShadows_Window(boldLabel);
                EditorGUILayout.Space(5);
                DrawAsync_Window(boldLabel);
                EditorGUILayout.Space(5);
                DrawLOD_Window(boldLabel);
                EditorGUILayout.Space(5);
                DwarMeshes_Window(boldLabel);

                EditorGUI.indentLevel--;
            }
        }
        void DwarMeshes_Window(GUIStyle boldLabel)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Meshes", boldLabel);
            QualitySettings.skinWeights = (SkinWeights)EditorGUILayout.EnumPopup("Skin Weights", QualitySettings.skinWeights);
            EditorGUI.indentLevel--;
        }
        void DrawLOD_Window(GUIStyle boldLabel)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Level of Detail", boldLabel);
            QualitySettings.lodBias = EditorGUILayout.FloatField("LOD Bias", QualitySettings.lodBias);
            QualitySettings.maximumLODLevel = EditorGUILayout.IntField("Maximum LOD Level", QualitySettings.maximumLODLevel);
            QualitySettings.enableLODCrossFade = EditorGUILayout.Toggle("LOD Cross Fade", QualitySettings.enableLODCrossFade);
            EditorGUI.indentLevel--;
        }
        void DrawAsync_Window(GUIStyle boldLabel)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Async Asset Upload", boldLabel);
            QualitySettings.asyncUploadTimeSlice = EditorGUILayout.IntField("Time Slice", QualitySettings.asyncUploadTimeSlice);
            QualitySettings.asyncUploadBufferSize = EditorGUILayout.IntField("Buffer Size", QualitySettings.asyncUploadBufferSize);
            QualitySettings.asyncUploadPersistentBuffer = EditorGUILayout.Toggle("Persistent Buffer", QualitySettings.asyncUploadPersistentBuffer);
            EditorGUI.indentLevel--;
        }
        void DrawShadows_Window(GUIStyle boldLabel)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Shadows", boldLabel);
            QualitySettings.shadowmaskMode = (ShadowmaskMode)EditorGUILayout.EnumPopup("Shadowmask Mode", QualitySettings.shadowmaskMode);
            EditorGUI.indentLevel--;
        }
        void DrawParticles_Window(GUIStyle boldLabel)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Particles", boldLabel);
            QualitySettings.particleRaycastBudget = EditorGUILayout.IntField("Particle Raycast Budget", QualitySettings.particleRaycastBudget);
            EditorGUI.indentLevel--;
        }
        void DrawTexture_Window(GUIStyle boldLabel)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Textures", boldLabel);
            string[] mipmapOptions = new[] {
                "0: Full Resolution",
                "1: Half Resolution",
                "2: Quarter Resolution",
                "3: Eighth Resolution"
};
            int[] mipmapValues = new[] { 0, 1, 2, 3, 4 };
            int selectedMipmapIndex = Mathf.Clamp(
                System.Array.IndexOf(mipmapValues, QualitySettings.globalTextureMipmapLimit), 0, mipmapValues.Length - 1);
            selectedMipmapIndex = EditorGUILayout.Popup("Global Texture Mipmap Limit", selectedMipmapIndex, mipmapOptions);
            QualitySettings.globalTextureMipmapLimit = mipmapValues[selectedMipmapIndex];
            EditorGUILayout.LabelField("TODO: Mipmap Limit Groups");
            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)EditorGUILayout.EnumPopup("Anisotropic Filtering", QualitySettings.anisotropicFiltering);
            EditorGUILayout.LabelField("TODO: Mipmap Streaming");
            EditorGUI.indentLevel--;
        }
        void DrawRendering_Window(GUIStyle boldLabel)
        {


            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Rendering", boldLabel);
            QualitySettings.renderPipeline = (RenderPipelineAsset)EditorGUILayout.ObjectField("Render Pipeline Asset", QualitySettings.renderPipeline, typeof(RenderPipelineAsset), false);
            QualitySettings.realtimeReflectionProbes = EditorGUILayout.Toggle("Realtime Reflection Probes", QualitySettings.realtimeReflectionProbes);
            QualitySettings.resolutionScalingFixedDPIFactor = EditorGUILayout.FloatField("Resolution Scaling Fixed DPI Factor", QualitySettings.resolutionScalingFixedDPIFactor);
            EditorGUILayout.LabelField("TODO: Realtime GI CPU Usage");
            string[] vSyncOptions = new[] { "Don't Sync", "Every VBlank", "Every Second VBlank" };
            int[] vSyncValues = new[] { 0, 1, 2 };
            int selectedIndex = System.Array.IndexOf(vSyncValues, QualitySettings.vSyncCount);
            selectedIndex = EditorGUILayout.Popup("VSync Count", selectedIndex, vSyncOptions);
            QualitySettings.vSyncCount = vSyncValues[selectedIndex];
            EditorGUI.indentLevel--;
        }
        #endregion
    }
}
