using System.Linq;
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
        }

        bool showGraphics_Window = false;
        void DrawMainLight_Window()
        {
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

                // Opción alternativa: mostrar la lista de calidad y su asset SRP
                EditorGUILayout.LabelField("Quality Levels (SRP Assets por calidad):", EditorStyles.boldLabel);

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
            }

            EditorGUI.indentLevel--;
        }

    }
}
