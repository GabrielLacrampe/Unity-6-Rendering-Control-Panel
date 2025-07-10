using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditorInternal;


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
        // Ajuste dinámico del ancho de la etiqueta
        EditorGUIUtility.labelWidth = Mathf.Clamp(position.width * 0.45f, 100, 220);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition); // INICIO SCROLL

        DrawCameraControls();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // Línea de separación
        DrawLightingControls();

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

    #region Lighting Controls
    bool showLighting = false;
    bool showMainLightWindow = false;
    bool showOtrasLuces = false;
    bool showEnvironmentWindow = false;
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

            DrawEnvironmentWindow();

            DrawMainLight();


            //TODO: componente de main light Lens Flare (SRP)

            DrawOtherLights();


            EditorGUI.indentLevel--;
        }
    }

    bool showGeneralSettings = false;
    bool showEmission = false;
    bool showShadows = false;
    bool showRenderingLight = false;
    void DrawMainLight()
    {
        EditorGUILayout.BeginVertical("box");
        showMainLightWindow = EditorGUILayout.Foldout(showMainLightWindow, "Main Light", true);
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel++;

        if (mainLight != null)
        {
            if (showMainLightWindow)
            {
                EditorGUILayout.BeginVertical("box");
                showGeneralSettings = EditorGUILayout.Foldout(showGeneralSettings, "General", true);
                EditorGUILayout.EndVertical();
                if (showGeneralSettings)
                {
                    EditorGUI.indentLevel++;

                    mainLight.type = (LightType)EditorGUILayout.EnumPopup("Tipo", mainLight.type);
                    mainLight.lightmapBakeType = (LightmapBakeType)EditorGUILayout.EnumPopup("Mode", mainLight.lightmapBakeType);

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.BeginVertical("box");
                showEmission = EditorGUILayout.Foldout(showEmission, "Emission", true);
                EditorGUILayout.EndVertical();
                if (showEmission)
                {
                    EditorGUI.indentLevel++;

                    //TODO falta Light Appearance
                    mainLight.color = EditorGUILayout.ColorField("Color", mainLight.color);
                    mainLight.intensity = EditorGUILayout.FloatField("Intensidad", mainLight.intensity);
                    mainLight.bounceIntensity = EditorGUILayout.FloatField("indirect Multiplier", mainLight.bounceIntensity);
                    mainLight.cookie = (Texture)EditorGUILayout.ObjectField("Cookie", mainLight.cookie, typeof(Texture), false);

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.BeginVertical("box");
                showRenderingLight = EditorGUILayout.Foldout(showRenderingLight, "Rendering", true);
                EditorGUILayout.EndVertical();
                if (showRenderingLight)
                {
                    EditorGUI.indentLevel++;

                    //TODO: falta Rendering Layers
                    mainLight.cullingMask = LayerMaskField("Culling Mask", mainLight.cullingMask);

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.BeginVertical("box");
                showShadows = EditorGUILayout.Foldout(showShadows, "Shadows", true);
                EditorGUILayout.EndVertical();
                if (showShadows)
                {
                    EditorGUI.indentLevel++;

                    mainLight.shadows = (LightShadows)EditorGUILayout.EnumPopup("Shadow Type", mainLight.shadows);

                    EditorGUI.indentLevel++;
                    mainLight.shadowAngle = EditorGUILayout.Slider("Baked Shadow Angle", mainLight.shadowAngle, 0f, 90f);

                    EditorGUILayout.LabelField("Realtime Shadows", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    mainLight.shadowStrength = EditorGUILayout.Slider("Strenght", mainLight.shadowStrength, 0f, 1f);
                    // TODO: falta shadow bias
                    mainLight.shadowNearPlane = EditorGUILayout.Slider("Shadow Near Plane", mainLight.shadowNearPlane, 0f, 10f);
                    //TODO: falta soft shadow quality
                    EditorGUI.indentLevel--;

                    //TODO: falta custom shadow layers
                    EditorGUI.indentLevel--;

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No se ha encontrado una luz direccional en la escena.", MessageType.Warning);
        }

        EditorGUI.indentLevel--;
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

    void DrawEnvironmentWindow()
    {
        EditorGUILayout.BeginVertical("box");
        showEnvironmentWindow = EditorGUILayout.Foldout(showEnvironmentWindow, "Environment", true);
        EditorGUILayout.EndVertical();
        if (showEnvironmentWindow)
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
                EditorGUILayout.HelpBox("Esta opción solo puede modificarse desde:\nWindow > Rendering > Lighting > Environment > Reflections > Compression",MessageType.Info);

                RenderSettings.reflectionIntensity = EditorGUILayout.Slider("Intensity Multiplier", RenderSettings.reflectionIntensity, 0f, 1f);
                RenderSettings.reflectionBounces = EditorGUILayout.IntSlider("Bounces", RenderSettings.reflectionBounces, 0, 4);

                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginVertical("box");
            showMenuOtherSettings = EditorGUILayout.Foldout(showMenuOtherSettings, "Other Settings", true);
            EditorGUILayout.EndVertical();
            if (showMenuOtherSettings)
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

    enum AmbientSource { Gradient, Skybox, Color }
    enum CameraProjection { Perspective, Orthographic }
    public enum ReflectionResolution
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
    TextureRequirement BoolToEnum(bool value) => value ? TextureRequirement.On : TextureRequirement.Off;
    bool EnumToBool(TextureRequirement value) => value == TextureRequirement.On;
    #endregion
}
