using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static Unity.Tutorials.Core.Editor.RichTextParser;

namespace RenderingControlPanel
{
    public class GraphicsSection : EditorSection
    {
        public override void Draw(GUIStyle boldLabel, bool showFocusedSettings)
        {
            DrawMainLight_Window();
            LogAllSerializedProperties();
        }

        bool showGraphics_Window = false;
        void DrawMainLight_Window()
        {
            Object graphicsAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0];
            

            EditorGUILayout.BeginVertical("box");
            showGraphics_Window = EditorGUILayout.Foldout(showGraphics_Window, "Graphics", true);
            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel++;

            if (showGraphics_Window)
            {
                // Obtener el asset actual del SRP
                var currentSRPAsset = GraphicsSettings.defaultRenderPipeline;
                var newSRPAsset = (RenderPipelineAsset)EditorGUILayout.ObjectField(
                    "Render Pipeline Asset",
                    currentSRPAsset,
                    typeof(RenderPipelineAsset),
                    false
                );
                // Si el usuario cambia el asset, actualizarlo
                if (newSRPAsset != currentSRPAsset)
                {
                    GraphicsSettings.defaultRenderPipeline = newSRPAsset;
                    EditorUtility.SetDirty(GraphicsSettings.GetGraphicsSettings());
                    Debug.Log("Render Pipeline Asset actualizado.");
                }

                EditorGUILayout.Space(10);

                ShaderStripping(graphicsAsset);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Shader Loading");
                GraphicsSettings.logWhenShaderIsCompiled = EditorGUILayout.Toggle(
                    "Log When Shader Is Compiled",
                    GraphicsSettings.logWhenShaderIsCompiled
                );

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Culling Settings");
                GraphicsSettings.cameraRelativeLightCulling = EditorGUILayout.Toggle(
                    "Lights",
                    GraphicsSettings.cameraRelativeLightCulling
                );
                GraphicsSettings.cameraRelativeShadowCulling = EditorGUILayout.Toggle(
                    "Shadows",
                    GraphicsSettings.cameraRelativeShadowCulling
                );

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("TODO: Shader Settings");
                EditorGUILayout.LabelField("TODO: Video");
                EditorGUILayout.LabelField("TODO: Always Included Shaders");
                EditorGUILayout.LabelField("TODO: Renderer Light Probe Selection");
                EditorGUILayout.LabelField("TODO: Preloaded Shaders");
                EditorGUILayout.LabelField("TODO: Preloaded Shaders after showing first scene");
                EditorGUILayout.LabelField("TODO: Currently traked shaders");


                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("TODO: Pipeline Specific Settings");


            }
            EditorGUI.indentLevel--;
        }
        void LogAllSerializedProperties()
        {
            var graphicsAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0];
            SerializedObject so = new SerializedObject(graphicsAsset);
            SerializedProperty prop = so.GetIterator();

            Debug.Log("=== List of Serialized Properties in GraphicsSettings.asset ===");

            while (prop.NextVisible(true))
            {
                Debug.Log($"{prop.propertyPath} ({prop.propertyType})");
            }
        }

        void ShaderStripping(Object graphicsAsset)
        {
            SerializedObject so = new SerializedObject(graphicsAsset);

            SerializedProperty m_LightmapStripping = so.FindProperty("m_LightmapStripping");
            SerializedProperty m_LightmapKeepPlain = so.FindProperty("m_LightmapKeepPlain");
            SerializedProperty m_LightmapKeepDirCombined = so.FindProperty("m_LightmapKeepDirCombined");
            SerializedProperty m_LightmapKeepDynamicPlain = so.FindProperty("m_LightmapKeepDynamicPlain");
            SerializedProperty m_LightmapKeepDynamicDirCombined = so.FindProperty("m_LightmapKeepDynamicDirCombined");
            SerializedProperty m_LightmapKeepShadowMask = so.FindProperty("m_LightmapKeepShadowMask");
            SerializedProperty m_LightmapKeepSubtractive = so.FindProperty("m_LightmapKeepSubtractive");

            SerializedProperty m_FogStripping = so.FindProperty("m_FogStripping");
            SerializedProperty m_FogKeepLinear = so.FindProperty("m_FogKeepLinear");
            SerializedProperty m_FogKeepExp = so.FindProperty("m_FogKeepExp");
            SerializedProperty m_FogKeepExp2 = so.FindProperty("m_FogKeepExp2");

            

            SerializedProperty m_InstancingStripping = so.FindProperty("m_InstancingStripping");
            SerializedProperty m_BrgStripping = so.FindProperty("m_BrgStripping");
            SerializedProperty m_PreloadShadersBatchTimeLimit = so.FindProperty("m_PreloadShadersBatchTimeLimit");


            EditorGUILayout.LabelField("Shader Stripping");
            EditorGUILayout.PropertyField(m_LightmapStripping, new GUIContent("Lightmap Modes"));

            if (m_LightmapStripping.enumValueIndex == 1) // 0 = Automatic, 1 = Custom
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_LightmapKeepPlain, new GUIContent("Baked Non-Directional"));
                EditorGUILayout.PropertyField(m_LightmapKeepDirCombined, new GUIContent("Baked Directional"));
                EditorGUILayout.PropertyField(m_LightmapKeepDynamicPlain, new GUIContent("Realtime Non-Directional"));
                EditorGUILayout.PropertyField(m_LightmapKeepDynamicDirCombined, new GUIContent("Realtime Directional"));
                EditorGUILayout.PropertyField(m_LightmapKeepShadowMask, new GUIContent("Baked Shadowmask"));
                EditorGUILayout.PropertyField(m_LightmapKeepSubtractive, new GUIContent("BakedSubstractive"));
                EditorGUILayout.LabelField("TODO:       Import From Current Scene");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Fog Modes");
            EditorGUILayout.PropertyField(m_FogStripping, new GUIContent("Fog Modes"));

            if (m_FogStripping.enumValueIndex == 1) // Custom
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_FogKeepLinear);
                EditorGUILayout.PropertyField(m_FogKeepExp);
                EditorGUILayout.PropertyField(m_FogKeepExp2);
                EditorGUILayout.LabelField("TODO:       Inport from Current Scene");
                EditorGUILayout.PropertyField(m_InstancingStripping);
                EditorGUILayout.PropertyField(m_BrgStripping);
                EditorGUI.indentLevel--;
            }
            //EditorGUILayout.PropertyField(m_PreloadShadersBatchTimeLimit);

            so.ApplyModifiedProperties();
        }
    }
}


// Opción alternativa: mostrar la lista de calidad y su asset SRP
//EditorGUILayout.LabelField("Quality Levels (SRP Assets por calidad):", EditorStyles.boldLabel);
/*
var qualityLevels = QualitySettings.names;
for (int i = 0; i < qualityLevels.Length; i++)
{
    RenderPipelineAsset qualitySRP = QualitySettings.GetRenderPipelineAssetAt(i);
    RenderPipelineAsset newQualitySRP = (RenderPipelineAsset)EditorGUILayout.ObjectField(
        $"[{i}] {qualityLevels[i]}",
        qualitySRP,
        typeof(RenderPipelineAsset),
        false
    );

}
*/