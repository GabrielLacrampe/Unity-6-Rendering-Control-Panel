using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RenderingControlPanel
{
    public class RenderingControlWindow : EditorWindow
    {
        private List<EditorSection> editorSections;
        public bool showFocusedSettings = false;

        [MenuItem("Tools/Rendering Control Panel")]
        static void ShowWindow()
        {
            GetWindow<RenderingControlWindow>("Rendering Control");
        }

        void OnEnable()
        {
            editorSections = new List<EditorSection>
            {
                new GraphicsSection(),
                new QualitySettingsSection(),
                new LightingSection(),
                new MainLightSection(),
                new MainCameraSection(),
            };
        }

        Vector2 scrollPosition;
        void OnGUI()
        {
            // EditorStyles
            EditorGUIUtility.labelWidth = Mathf.Clamp(position.width * 0.45f, 100, 220);
            GUIStyle boldLabel = new(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold
            };
            // Window
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // INICIO SCROLL
            foreach (var section in editorSections)
            {
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                section.Draw(boldLabel, showFocusedSettings);
            }
            GUILayout.FlexibleSpace();
            showFocusedSettings = EditorGUILayout.ToggleLeft("Focus Settings", showFocusedSettings);
            EditorGUILayout.EndScrollView(); // FIN SCROLL
        }
    }


    enum AmbientSource { Gradient, Skybox, Color }
    enum CameraProjection { Perspective, Orthographic }
    enum ReflectionResolution
    {
        _16 = 16,
        _32 = 32,
        _64 = 64,
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048
    }
    enum CleanedClearFlags
    {
        Skybox = CameraClearFlags.Skybox,
        SolidColor = CameraClearFlags.SolidColor,
        Uninitialized = CameraClearFlags.Nothing
    }
    enum TextureRequirement
    {
        Off,
        On
        // En el futuro podrías añadir "Auto" si tuviera sentido
    }
}
