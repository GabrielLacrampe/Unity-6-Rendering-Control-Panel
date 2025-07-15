using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace RenderingControlPanel
{
    public class GraphicsSection : EditorSection
    {
        public override void Draw(GUIStyle boldLabel, bool showFocusedSettings)
        {
            DrawMainLight_Window();
            //LogAllSerializedProperties();
        }

        bool showGraphics_Window = false;
        void DrawMainLight_Window()
        {
            Object graphicsAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0];
            SerializedObject so = new(graphicsAsset);

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
                ShaderStripping(so);

                EditorGUILayout.Space(10);
                FogPropertys(so);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Shader Loading");
                GraphicsSettings.logWhenShaderIsCompiled = EditorGUILayout.Toggle(
                    "Log Shader Compilation",
                    GraphicsSettings.logWhenShaderIsCompiled
                );

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Culling Settings");
                EditorGUILayout.LabelField("Camera-Relative Culling");
                EditorGUI.indentLevel++;
                GraphicsSettings.cameraRelativeLightCulling = EditorGUILayout.Toggle(
                    "Lights",
                    GraphicsSettings.cameraRelativeLightCulling
                );
                GraphicsSettings.cameraRelativeShadowCulling = EditorGUILayout.Toggle(
                    "Shadows",
                    GraphicsSettings.cameraRelativeShadowCulling
                );
                EditorGUI.indentLevel--;

                EditorGUILayout.Space(10);
                ShaderSettings(so);

                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("TODO: Pipeline Specific Settings");

                so.ApplyModifiedProperties();
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

        void ShaderStripping(SerializedObject so)
        {
            SerializedProperty m_LightmapStripping = so.FindProperty("m_LightmapStripping");
            SerializedProperty m_LightmapKeepPlain = so.FindProperty("m_LightmapKeepPlain");
            SerializedProperty m_LightmapKeepDirCombined = so.FindProperty("m_LightmapKeepDirCombined");
            SerializedProperty m_LightmapKeepDynamicPlain = so.FindProperty("m_LightmapKeepDynamicPlain");
            SerializedProperty m_LightmapKeepDynamicDirCombined = so.FindProperty("m_LightmapKeepDynamicDirCombined");
            SerializedProperty m_LightmapKeepShadowMask = so.FindProperty("m_LightmapKeepShadowMask");
            SerializedProperty m_LightmapKeepSubtractive = so.FindProperty("m_LightmapKeepSubtractive");

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
        }

        static void ShaderSettings(SerializedObject so)
        {
            SerializedProperty m_VideoShadersIncludeMode = so.FindProperty("m_VideoShadersIncludeMode");
            SerializedProperty m_AlwaysIncludedShaders = so.FindProperty("m_AlwaysIncludedShaders");
            SerializedProperty m_PreloadedShaders = so.FindProperty("m_PreloadedShaders");
            SerializedProperty m_ScreenSpaceShadows = so.FindProperty("m_ScreenSpaceShadows");

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Shader Settings");
            EditorGUILayout.PropertyField(m_VideoShadersIncludeMode, new GUIContent("Video"));
            EditorGUILayout.PropertyField(m_AlwaysIncludedShaders, new GUIContent("Always Included Shaders"));
            EditorGUILayout.LabelField("TODO: Renderer Light Probe Selection");
            EditorGUILayout.PropertyField(m_PreloadedShaders, new GUIContent("Preloaded Shaders"));
            EditorGUILayout.LabelField("TODO: Preloaded Shaders after showing first scene");
            EditorGUILayout.LabelField("TODO: Currently traked: ");
            EditorGUILayout.LabelField("TODO:                       Save to asset... / Clear");
            EditorGUILayout.PropertyField(m_ScreenSpaceShadows);
        }

        static void FogPropertys(SerializedObject so)
        {
            SerializedProperty m_FogStripping = so.FindProperty("m_FogStripping");
            SerializedProperty m_FogKeepLinear = so.FindProperty("m_FogKeepLinear");
            SerializedProperty m_FogKeepExp = so.FindProperty("m_FogKeepExp");
            SerializedProperty m_FogKeepExp2 = so.FindProperty("m_FogKeepExp2");
            SerializedProperty m_InstancingStripping = so.FindProperty("m_InstancingStripping");
            SerializedProperty m_BrgStripping = so.FindProperty("m_BrgStripping");

            EditorGUILayout.PropertyField(m_FogStripping, new GUIContent("Fog Modes"));
            if (m_FogStripping.enumValueIndex == 1)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_FogKeepLinear, new GUIContent("Linear"));
                EditorGUILayout.PropertyField(m_FogKeepExp, new GUIContent("Exponential"));
                EditorGUILayout.PropertyField(m_FogKeepExp2, new GUIContent("Exponential Squared"));
                EditorGUILayout.LabelField("TODO:       Inport from Current Scene");
                EditorGUILayout.PropertyField(m_InstancingStripping, new GUIContent("Instancing Variants"));
                EditorGUILayout.PropertyField(m_BrgStripping, new GUIContent("BatchRendererGroup Variants"));
                EditorGUI.indentLevel--;
            }
        }
    }
}
