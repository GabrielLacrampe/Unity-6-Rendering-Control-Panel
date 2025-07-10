using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditorInternal;


public class RenderingControlWindow : EditorWindow
{
    Color ambientLight;
    Light mainLight;
    Camera mainCamera;

    #region Setup
    [MenuItem("Tools/Rendering Control Panel")]
    static void ShowWindow()
    {
        GetWindow<RenderingControlWindow>("Rendering Control");
    }

    void OnEnable()
    {
        mainCamera = Camera.main;
        ambientLight = RenderSettings.ambientLight;
        mainLight = FindDirectionalLight();
    }


    bool showFocusedSettings = false;
    Vector2 scrollPosition; // Añade esta variable para el scroll
    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // INICIO SCROLL

        DrawCameraControls();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // Línea de separación
        DrawLightingControls();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // Línea de separación
        DrawSkyboxControls();

        GUILayout.FlexibleSpace();

        showFocusedSettings = EditorGUILayout.Toggle("Focus Settings", showFocusedSettings);

        if (GUILayout.Button("Refrescar datos"))
        {
            mainLight = FindDirectionalLight();
            ambientLight = RenderSettings.ambientLight;
        }

        EditorGUILayout.EndScrollView(); // FIN SCROLL
    }
    #endregion

    /* TODO: implementar las siguientes secciones:
     * 
     * Parámetros globales de iluminación
     * Menú Window > Rendering > Lighting (Pestaña Scene y Environment)
     * 
     * el modo de luz ambiental (Skybox, Color, Gradient), la intensidad y el color.
     * Sección Environment Reflections
     * 
     * Parámetros globales de sombras
     * Edit > Project Settings > Quality
     * seleccionar el nivel de calidad y, para cada nivel, ajustar:
     *      Shadows (Calidad de sombras)
     *      Shadow Distance (Distancia de sombra)
     *      Shadow Resolution (Resolución de sombra)
     *      
     * 
     */

    #region Lighting Controls
    bool showLighting = false;
    bool showMainLight = false;
    bool showOtrasLuces = false;
    bool showMainMenuEnvironment = false;
    bool showMenuEnvironment = false;
    bool showMenuOtherSettings = false;

    void DrawLightingControls()
    {
        EditorGUILayout.BeginVertical("box");
        showLighting = EditorGUILayout.Foldout(showLighting, "Lighting", true);
        EditorGUILayout.EndVertical();

        if (showLighting)
        {
            EditorGUI.indentLevel++;

            DrawGlobalParameters();

            DrawMainLight();

            DrawOtherLights();

            EditorGUI.indentLevel--;
        }
    }

    void DrawOtherLights()
    {
        var allLights = GameObject.FindObjectsOfType<Light>().Where(l => l != mainLight).ToArray();
        if (allLights.Length > 0)
        {
            EditorGUILayout.BeginVertical("box");
            showOtrasLuces = EditorGUILayout.Foldout(showOtrasLuces, "Otras luces en la escena:", true);
            EditorGUILayout.EndVertical();

            if (showOtrasLuces)
            {
                EditorGUI.indentLevel++;
                foreach (var light in allLights)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField(light.name, EditorStyles.miniBoldLabel);
                    light.type = (LightType)EditorGUILayout.EnumPopup("Tipo", light.type);
                    light.color = EditorGUILayout.ColorField("Color", light.color);
                    light.intensity = EditorGUILayout.Slider("Intensidad", light.intensity, 0f, 8f);
                    light.shadows = (LightShadows)EditorGUILayout.EnumPopup("Sombras", light.shadows);
                    light.shadowStrength = EditorGUILayout.Slider("Fuerza de Sombra", light.shadowStrength, 0f, 1f);

                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space(5);
                }
                EditorGUI.indentLevel--;
            }
        }
    }

    void DrawMainLight()
    {
        EditorGUILayout.BeginVertical("box");
        showMainLight = EditorGUILayout.Foldout(showMainLight, "Main Light:", true);
        EditorGUILayout.EndVertical();
        if (mainLight != null)
        {
            if (showMainLight)
            {
                mainLight.color = EditorGUILayout.ColorField("Color", mainLight.color);
                mainLight.intensity = EditorGUILayout.Slider("Intensidad", mainLight.intensity, 0f, 8f);
                mainLight.transform.rotation = Quaternion.Euler(
                    EditorGUILayout.Vector3Field("Rotación", mainLight.transform.rotation.eulerAngles)
                );
                mainLight.shadows = (LightShadows)EditorGUILayout.EnumPopup("Sombras", mainLight.shadows);
                mainLight.shadowStrength = EditorGUILayout.Slider("Fuerza de Sombra", mainLight.shadowStrength, 0f, 1f);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No se ha encontrado una luz direccional en la escena.", MessageType.Warning);
        }
    }

    void DrawGlobalParameters()
    {
        EditorGUILayout.BeginVertical("box");
        showMainMenuEnvironment = EditorGUILayout.Foldout(showMainMenuEnvironment, "Environment", true);
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel++;
        if (showMainMenuEnvironment)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical("box");
            showMenuEnvironment = EditorGUILayout.Foldout(showMenuEnvironment, "Environment", true);
            EditorGUILayout.EndVertical();

            if (showMenuEnvironment)
            {
                EditorGUI.indentLevel++;

                RenderSettings.skybox = (Material)EditorGUILayout.ObjectField("Skybox Material", RenderSettings.skybox, typeof(Material), false);
                RenderSettings.sun = (Light)EditorGUILayout.ObjectField("Sun Light", RenderSettings.sun, typeof(Light), true);


            }

            EditorGUILayout.BeginVertical("box");
            showMenuOtherSettings = EditorGUILayout.Foldout(showMenuOtherSettings, "Other Settings", true);
            EditorGUILayout.EndVertical();

            if (showMenuOtherSettings)
            {
                RenderSettings.fog = EditorGUILayout.Toggle("Enable Fog", RenderSettings.fog);
                if (RenderSettings.fog)
                {
                    EditorGUI.indentLevel++;

                    RenderSettings.fogColor = EditorGUILayout.ColorField("Color", RenderSettings.fogColor);
                    RenderSettings.fogMode = (FogMode)EditorGUILayout.EnumPopup("Mode", RenderSettings.fogMode);
                    RenderSettings.fogDensity = EditorGUILayout.Slider("Density?", RenderSettings.fogDensity, 0f, 1f);
                    RenderSettings.fogStartDistance = EditorGUILayout.FloatField("Start", RenderSettings.fogStartDistance);
                    RenderSettings.fogEndDistance = EditorGUILayout.FloatField("End", RenderSettings.fogEndDistance);

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.indentLevel--;
        }

        #region Pendiente organizar
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // Línea de separación

        RenderSettings.ambientMode = (AmbientMode)EditorGUILayout.EnumPopup("Ambient Mode", RenderSettings.ambientMode);
        RenderSettings.ambientIntensity = EditorGUILayout.Slider("Ambient Intensity", RenderSettings.ambientIntensity, 0f, 8f);
        RenderSettings.ambientLight = EditorGUILayout.ColorField("Ambient Light", RenderSettings.ambientLight);
        RenderSettings.reflectionIntensity = EditorGUILayout.Slider("Reflection Intensity", RenderSettings.reflectionIntensity, 0f, 1f);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Sombras globales:", EditorStyles.boldLabel);
        QualitySettings.shadows = (UnityEngine.ShadowQuality)EditorGUILayout.EnumPopup("Calidad de sombras", QualitySettings.shadows);
        QualitySettings.shadowDistance = EditorGUILayout.FloatField("Distancia de sombra", QualitySettings.shadowDistance);
        QualitySettings.shadowResolution = (UnityEngine.ShadowResolution)EditorGUILayout.EnumPopup("Resolución de sombra", QualitySettings.shadowResolution);
        #endregion

        EditorGUI.indentLevel--;
    }


    /* pendiente corregir y buscar donde implementar
    RenderSettings.defaultReflectionMode = (DefaultReflectionMode)EditorGUILayout.EnumPopup("Default Reflection Mode", RenderSettings.defaultReflectionMode);
    if (RenderSettings.defaultReflectionMode == DefaultReflectionMode.Custom)
    {
        RenderSettings.customReflection = (Cubemap)EditorGUILayout.ObjectField("Custom Reflection", RenderSettings.customReflection, typeof(Cubemap), false);
    }
    RenderSettings.reflectionBounces = EditorGUILayout.IntSlider("Reflection Bounces", RenderSettings.reflectionBounces, 0, 4);
    RenderSettings.reflectionProbeUsage = (ReflectionProbeUsage)EditorGUILayout.EnumPopup("Reflection Probe Usage", RenderSettings.reflectionProbeUsage);
    RenderSettings.defaultReflectionResolution = EditorGUILayout.IntField("Default Reflection Resolution", RenderSettings.defaultReflectionResolution);
    RenderSettings.lightProbeUsage = (LightProbeUsage)EditorGUILayout.EnumPopup("Light Probe Usage", RenderSettings.lightProbeUsage);
    RenderSettings.lightProbeProxyVolumeResolution = EditorGUILayout.IntField("Light Probe Proxy Volume Resolution", RenderSettings.lightProbeProxyVolumeResolution);
    RenderSettings.lightProbeProxyVolumeImportance = EditorGUILayout.FloatField("Light Probe Proxy Volume Importance", RenderSettings.lightProbeProxyVolumeImportance);
    RenderSettings.lightProbeProxyVolumeAnchor = (Transform)EditorGUILayout.ObjectField("Light Probe Proxy Volume Anchor", RenderSettings.lightProbeProxyVolumeAnchor, typeof(Transform), true);
    RenderSettings.lightProbeProxyVolumeOverride = (GameObject)EditorGUILayout.ObjectField("Light Probe Proxy Volume Override", RenderSettings.lightProbeProxyVolumeOverride, typeof(GameObject), true);
    */
    #endregion

    #region Skybox Controls

    bool showSkybox = false;
    void DrawSkyboxControls()
    {
        EditorGUILayout.BeginVertical("box");
        showSkybox = EditorGUILayout.Foldout(showSkybox, "Skybox y Ambient", true);
        EditorGUILayout.EndVertical();

        if (showSkybox)
        {



            EditorGUI.indentLevel--;
        }
    }

    #endregion

    #region Camera Controls
    bool showCamera = true;
    bool showProjection = false;
    bool showRendering = false;
    bool showStack = false;
    bool showEnvironment = false;
    bool showOutput = false;
    void DrawCameraControls()
    {
        EditorGUILayout.BeginVertical("box"); // <-- Empieza el marco
        showCamera = EditorGUILayout.Foldout(showCamera, "Main Camera", true);
        EditorGUILayout.EndVertical(); // <-- Termina el marco

        if (!showCamera) return;
        if (mainCamera == null)
        {
            EditorGUILayout.HelpBox("No se ha encontrado una cámara con la etiqueta 'MainCamera'.", MessageType.Warning);
            return;
        }

        EditorGUI.indentLevel++;

        //TODO falta Render Type

        #region Proyección
        EditorGUILayout.BeginVertical("box"); // <-- Empieza el marco
        showProjection = EditorGUILayout.Foldout(showProjection, "Projection", true);
        EditorGUILayout.EndVertical(); // <-- Termina el marco

        if (showProjection)
        {
            EditorGUI.indentLevel++;
            CameraProjection projection = mainCamera.orthographic ? CameraProjection.Orthographic : CameraProjection.Perspective;
            projection = (CameraProjection)EditorGUILayout.EnumPopup("Projection", projection);
            mainCamera.orthographic = projection == CameraProjection.Orthographic;

            EditorGUI.indentLevel++;

            // Field of View Axis
            // TODO falta field of view axis horizontal, vertical
            if (!mainCamera.orthographic)
            {
                // Field of View (solo en perspectiva)
                mainCamera.fieldOfView = EditorGUILayout.Slider("Field of View", mainCamera.fieldOfView, 1f, 179f);
            }
            else
            {
                // Size (solo en ortográfica)
                mainCamera.orthographicSize = EditorGUILayout.FloatField("Orthographic Size", mainCamera.orthographicSize);
            }


            // Clipping Planes
            EditorGUILayout.LabelField("Clipping Planes", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            mainCamera.nearClipPlane = EditorGUILayout.FloatField("Near", mainCamera.nearClipPlane);
            mainCamera.farClipPlane = EditorGUILayout.FloatField("Far", mainCamera.farClipPlane);
            EditorGUI.indentLevel--;

#if UNITY_2020_1_OR_NEWER
            // Physical Camera (solo en Perspective)
            // TODO terminar sección de cámara física
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
        #endregion

        Rendering();

        #region  Stack
        EditorGUILayout.BeginVertical("box"); // <-- Empieza el marco
        showStack = EditorGUILayout.Foldout(showStack, "Stack", true);
        EditorGUILayout.EndVertical(); // <-- Termina el marco

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

                if (GUILayout.Button("Añadir cámara overlay"))
                {
                    additionalData.cameraStack.Add(null);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Stack solo disponible en cámaras con Render Type: Base", MessageType.Info);
            }

            EditorGUI.indentLevel--;
        }
        #endregion

        Environment();

        #region  Output
        EditorGUILayout.BeginVertical("box"); // <-- Empieza el marco
        showOutput = EditorGUILayout.Foldout(showOutput, "Output", true);
        EditorGUILayout.EndVertical(); // <-- Termina el marco

        if (showOutput)
        {
            EditorGUI.indentLevel++;

            mainCamera.targetDisplay = EditorGUILayout.IntSlider("Target Display", mainCamera.targetDisplay, 0, Display.displays.Length - 1);
            mainCamera.depth = EditorGUILayout.FloatField("Depth", mainCamera.depth);

            var additionalData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
            if (additionalData != null)
            {
                //additionalData.outputCamera = (Camera)EditorGUILayout.ObjectField("Output Camera", additionalData.outputCamera, typeof(Camera), true);
            }

            EditorGUI.indentLevel--;
        }

        #endregion

        EditorGUI.indentLevel--;
    }

    void Rendering()
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

                // Obtenemos el valor de defaultRendererIndex usando SerializedObject
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

                // Agregamos el resto de renderers con su índice
                for (int i = 0; i < list.Length; i++)
                {
                    string name = list[i] != null ? list[i].name : "Unnamed Renderer";
                    rendererNames[i + 1] = $"{i}: {name}";
                }

                // Traducimos rendererProp.intValue a índice del popup
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
    void Environment()
    {
        EditorGUILayout.BeginVertical("box"); // <-- Empieza el marco
        showEnvironment = EditorGUILayout.Foldout(showEnvironment, "Environment", true);
        EditorGUILayout.EndVertical(); // <-- Termina el marco

        if (showEnvironment)
        {
            EditorGUI.indentLevel++;

            // Mapear el clearFlags actual al enum filtrado
            CleanedClearFlags filteredClearFlags = (CleanedClearFlags)mainCamera.clearFlags;

            filteredClearFlags = (CleanedClearFlags)EditorGUILayout.EnumPopup("Background Type", filteredClearFlags);

            // Aplicar el cambio a la cámara (cast a CameraClearFlags original)
            mainCamera.clearFlags = (CameraClearFlags)filteredClearFlags;

            // Background color
            if (mainCamera.clearFlags == CameraClearFlags.SolidColor)
            {
                EditorGUI.indentLevel++;
                mainCamera.backgroundColor = EditorGUILayout.ColorField("Background", mainCamera.backgroundColor);

                EditorGUI.indentLevel--;
            }

            // Obtener datos de cámara extendidos de URP
            var additionalData = mainCamera.GetComponent<UniversalAdditionalCameraData>();
            if (additionalData != null)
            {
                if (showFocusedSettings) return;
                EditorGUILayout.LabelField("Volumes");

                EditorGUI.indentLevel++;
                // Obtener el modo actual
                VolumeFrameworkUpdateMode currentMode = mainCamera.GetVolumeFrameworkUpdateMode();

                // Mostrar selector
                currentMode = (VolumeFrameworkUpdateMode)EditorGUILayout.EnumPopup("Update Mode", currentMode);

                // Aplicar el modo seleccionado
                if (EditorGUI.EndChangeCheck())
                    mainCamera.SetVolumeFrameworkUpdateMode(currentMode);
                additionalData.volumeLayerMask = LayerMaskField("Volume Mask", additionalData.volumeLayerMask);
                additionalData.volumeTrigger = (Transform)EditorGUILayout.ObjectField("Volume Trigger", additionalData.volumeTrigger, typeof(Transform), true);

                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
        }
    }

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
    #endregion

    #region Tools

    // TODO: falta organizar el código en secciones más pequeñas y manejables

    Light FindDirectionalLight()
    {
        return GameObject.FindObjectsOfType<Light>()
            .FirstOrDefault(l => l.type == LightType.Directional);
    }
    enum CameraProjection
    {
        Perspective,
        Orthographic
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
    TextureRequirement BoolToEnum(bool value) => value ? TextureRequirement.On : TextureRequirement.Off;
    bool EnumToBool(TextureRequirement value) => value == TextureRequirement.On;
    #endregion
}
