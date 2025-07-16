using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderingControlPanel
{
    public class MainCameraSection : EditorSection
    {
        bool showFocusedSettings = false;

        bool showCamera = false;
        bool showProjection = false;
        bool showRendering = false;
        bool showStack = false;
        bool showEnvironment = false;
        bool showOutput = false;

        public override void Draw(GUIStyle boldLabel, bool _showFocusedSettings)
        {
            DrawMainCamera();
            showFocusedSettings = _showFocusedSettings;
        }

        void DrawMainCamera()
        {
            Camera mainCamera = Camera.main;
            EditorGUILayout.BeginVertical("box");
            showCamera = EditorGUILayout.Foldout(showCamera, "Main Camera", true);
            EditorGUILayout.EndVertical();

            if (!showCamera) return;

            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("TODO: Render Type");

            Projection(mainCamera);
            Rendering(mainCamera);
            Stack(mainCamera);
            Environment(mainCamera, showFocusedSettings);
            Output(mainCamera);

            EditorGUI.indentLevel--;
        }
        void Projection(Camera mainCamera)
        {
            EditorGUILayout.BeginVertical("box");
            showProjection = EditorGUILayout.Foldout(showProjection, "Projection", true);
            EditorGUILayout.EndVertical();

            if (showProjection)
            {
                EditorGUI.indentLevel++;
                CameraProjection projection = mainCamera.orthographic ? CameraProjection.Orthographic : CameraProjection.Perspective;
                projection = (CameraProjection)EditorGUILayout.EnumPopup("Projection", projection);
                mainCamera.orthographic = projection == CameraProjection.Orthographic;

                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("TODO: field of view axis horizontal, vertical");
                if (!mainCamera.orthographic)
                {
                    mainCamera.fieldOfView = EditorGUILayout.Slider("Field of View", mainCamera.fieldOfView, 1f, 179f);
                }
                else
                {
                    mainCamera.orthographicSize = EditorGUILayout.FloatField("Orthographic Size", mainCamera.orthographicSize);
                }

                EditorGUILayout.LabelField("Clipping Planes", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                mainCamera.nearClipPlane = EditorGUILayout.FloatField("Near", mainCamera.nearClipPlane);
                mainCamera.farClipPlane = EditorGUILayout.FloatField("Far", mainCamera.farClipPlane);
                EditorGUI.indentLevel--;

#if UNITY_2020_1_OR_NEWER

                EditorGUILayout.LabelField("TODO: Phisical Camera");
                mainCamera.usePhysicalProperties = !mainCamera.orthographic && EditorGUILayout.Toggle("Physical Camera", mainCamera.usePhysicalProperties);

                if (mainCamera.usePhysicalProperties && !mainCamera.orthographic)
                {
                    mainCamera.focalLength = EditorGUILayout.FloatField("Focal Length", mainCamera.focalLength);
                    mainCamera.sensorSize = EditorGUILayout.Vector2Field("Sensor Size", mainCamera.sensorSize);
                    mainCamera.lensShift = EditorGUILayout.Vector2Field("Lens Shift", mainCamera.lensShift);
                }
#endif
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
            }
        }
        void Stack(Camera mainCamera)
        {
            EditorGUILayout.BeginVertical("box");
            showStack = EditorGUILayout.Foldout(showStack, "Stack", true);
            EditorGUILayout.EndVertical();

            if (showStack)
            {
                EditorGUI.indentLevel++;

                var additionalData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
                if (additionalData != null && additionalData.renderType == CameraRenderType.Base)
                {
                    EditorGUILayout.LabelField("Camera Stack", EditorStyles.boldLabel);

                    for (int i = 0; i < additionalData.cameraStack.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        additionalData.cameraStack[i] = (Camera)EditorGUILayout.ObjectField($"Overlay {i}", additionalData.cameraStack[i], typeof(Camera), true);
                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            additionalData.cameraStack.RemoveAt(i);
                            i--;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("Add Camera Overlay"))
                    {
                        additionalData.cameraStack.Add(null);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Stack only available with Render Type: Base", MessageType.Info);
                }

                EditorGUI.indentLevel--;
            }
        }
        void Output(Camera mainCamera)
        {
            EditorGUILayout.BeginVertical("box");
            showOutput = EditorGUILayout.Foldout(showOutput, "Output", true);
            EditorGUILayout.EndVertical();

            if (showOutput)
            {
                EditorGUI.indentLevel++;

                mainCamera.targetDisplay = EditorGUILayout.IntSlider("Target Display", mainCamera.targetDisplay, 0, Display.displays.Length - 1);
                mainCamera.depth = EditorGUILayout.FloatField("Depth", mainCamera.depth);

                var additionalData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
                if (additionalData != null)
                {
                    EditorGUILayout.LabelField("TODO: additionalData.outputCamera ");
                }

                EditorGUI.indentLevel--;
            }
        }
        void Rendering(Camera mainCamera)
        {
            EditorGUILayout.BeginVertical("box");
            showRendering = EditorGUILayout.Foldout(showRendering, "Rendering", true);
            EditorGUILayout.EndVertical();

            if (showRendering)
            {
                EditorGUI.indentLevel++;

                var additionalData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
                if (additionalData == null)
                {
                    EditorGUILayout.HelpBox("No se ha encontrado el componente UniversalAdditionalCameraData.", MessageType.Warning);
                    EditorGUI.indentLevel--;
                    return;
                }

                #region Renderer
                SerializedObject camDataSO = new SerializedObject(additionalData);
                SerializedProperty rendererProp = camDataSO.FindProperty("m_RendererIndex");

                var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

                string[] rendererNames = null;
                int popupIndex = 0;

                if (urpAsset != null && urpAsset.rendererDataList != null)
                {
                    var list = urpAsset.rendererDataList;
                    rendererNames = new string[list.Length + 1]; // +1 para el Default

                    int defaultRendererIndex = 0; // fallback

                    SerializedObject urpSerialized = new SerializedObject(urpAsset);
                    SerializedProperty defaultRendererProp = urpSerialized.FindProperty("m_DefaultRendererIndex");

                    if (defaultRendererProp != null)
                        defaultRendererIndex = defaultRendererProp.intValue;

                    string defaultRendererName = (defaultRendererIndex >= 0 && defaultRendererIndex < urpAsset.rendererDataList.Length
                    && urpAsset.rendererDataList[defaultRendererIndex] != null)
                    ? urpAsset.rendererDataList[defaultRendererIndex].name
                    : "Unknown";

                    rendererNames[0] = $"Default Renderer ({defaultRendererName})";

                    for (int i = 0; i < list.Length; i++)
                    {
                        string name = list[i] != null ? list[i].name : "Unnamed Renderer";
                        rendererNames[i + 1] = $"{i}: {name}";
                    }

                    popupIndex = rendererProp.intValue == -1 ? 0 : rendererProp.intValue + 1;
                    popupIndex = Mathf.Clamp(popupIndex, 0, rendererNames.Length - 1);
                }

                if (rendererNames != null && rendererProp != null)
                {
                    int selected = EditorGUILayout.Popup("Renderer", popupIndex, rendererNames);

                    int newValue = selected == 0 ? -1 : selected - 1;

                    if (newValue != rendererProp.intValue)
                    {
                        rendererProp.intValue = newValue;
                        camDataSO.ApplyModifiedProperties();
                    }
                }
                #endregion

                EditorGUI.BeginChangeCheck();
                bool newPostProcessing = EditorGUILayout.Toggle("Post Processing", additionalData.renderPostProcessing);
                if (EditorGUI.EndChangeCheck())
                {
                    additionalData.renderPostProcessing = newPostProcessing;
                }

                #region Anti-aliasing
                additionalData.antialiasing = (AntialiasingMode)EditorGUILayout.EnumPopup("Anti-aliasing", additionalData.antialiasing);
                EditorGUI.indentLevel++;


                //TemporalAntiAliasing
                if (additionalData.antialiasing == AntialiasingMode.TemporalAntiAliasing)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.HelpBox(
                            "Las opciones de calidad y nitidez no se pueden modificar desde aquí.",
                            MessageType.Info
                        );

                    EditorGUI.indentLevel--;
                }
                //SubpixelMorphologicalAntiAliasing
                if (additionalData.antialiasing == AntialiasingMode.SubpixelMorphologicalAntiAliasing)
                {
                    EditorGUI.indentLevel++;
                    additionalData.antialiasingQuality = (AntialiasingQuality)EditorGUILayout.EnumPopup("Quality", additionalData.antialiasingQuality);

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
                #endregion


                #region Extra rendering flags
                additionalData.stopNaN = EditorGUILayout.Toggle("Stop NaNs", additionalData.stopNaN);
                additionalData.dithering = EditorGUILayout.Toggle("Dithering", additionalData.dithering);

                // Shadows
                additionalData.renderShadows = EditorGUILayout.Toggle("Render Shadows", additionalData.renderShadows);

                mainCamera.depth = EditorGUILayout.FloatField("Priority", mainCamera.depth);


                // Opaque / Depth texture
                additionalData.requiresColorTexture = EnumToBool((TextureRequirement)EditorGUILayout.EnumPopup("Opaque Texture", BoolToEnum(additionalData.requiresColorTexture)));
                additionalData.requiresDepthTexture = EnumToBool((TextureRequirement)EditorGUILayout.EnumPopup("Depth Texture", BoolToEnum(additionalData.requiresDepthTexture)));


                // Culling
                mainCamera.cullingMask = LayerMaskField("Culling Mask", mainCamera.cullingMask);
                mainCamera.useOcclusionCulling = EditorGUILayout.Toggle("Occlusion Culling", mainCamera.useOcclusionCulling);
                #endregion

                EditorGUI.indentLevel--;
            }
        }
        void Environment(Camera mainCamera, bool showFocusedSettings)
        {
            EditorGUILayout.BeginVertical("box");
            showEnvironment = EditorGUILayout.Foldout(showEnvironment, "Environment", true);
            EditorGUILayout.EndVertical();

            if (showEnvironment)
            {
                EditorGUI.indentLevel++;

                CleanedClearFlags filteredClearFlags = (CleanedClearFlags)mainCamera.clearFlags;

                filteredClearFlags = (CleanedClearFlags)EditorGUILayout.EnumPopup("Background Type", filteredClearFlags);

                mainCamera.clearFlags = (CameraClearFlags)filteredClearFlags;

                // Background color
                if (mainCamera.clearFlags == CameraClearFlags.SolidColor)
                {
                    EditorGUI.indentLevel++;
                    mainCamera.backgroundColor = EditorGUILayout.ColorField("Background", mainCamera.backgroundColor);

                    EditorGUI.indentLevel--;
                }

                var additionalData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
                if (additionalData != null)
                {
                    if (showFocusedSettings) return;
                    EditorGUILayout.LabelField("Volumes");

                    EditorGUI.indentLevel++;
                    VolumeFrameworkUpdateMode currentMode = mainCamera.GetVolumeFrameworkUpdateMode();

                    currentMode = (VolumeFrameworkUpdateMode)EditorGUILayout.EnumPopup("Update Mode", currentMode);

                    if (EditorGUI.EndChangeCheck())
                        mainCamera.SetVolumeFrameworkUpdateMode(currentMode);
                    additionalData.volumeLayerMask = LayerMaskField("Volume Mask", additionalData.volumeLayerMask);
                    additionalData.volumeTrigger = (Transform)EditorGUILayout.ObjectField("Volume Trigger", additionalData.volumeTrigger, typeof(Transform), true);

                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        TextureRequirement BoolToEnum(bool value) => value ? TextureRequirement.On : TextureRequirement.Off;
        bool EnumToBool(TextureRequirement value) => value == TextureRequirement.On;

        static LayerMask LayerMaskField(string label, LayerMask selected)
        {
            var layers = InternalEditorUtility.layers;
            var layerNumbers = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++)
                layerNumbers[i] = LayerMask.NameToLayer(layers[i]);

            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Length; i++)
                if (((1 << layerNumbers[i]) & selected.value) > 0)
                    maskWithoutEmpty |= (1 << i);

            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

            int mask = 0;
            for (int i = 0; i < layerNumbers.Length; i++)
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);

            selected.value = mask;
            return selected;
        }
    }
}
