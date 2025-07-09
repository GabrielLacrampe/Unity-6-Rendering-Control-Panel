using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditorInternal;


public class RenderingControlWindow : EditorWindow
{
    bool showFocusedSettings = false;

    bool showLighting = true;
    bool showSkybox = true;


    Color ambientColor;
    Light mainLight;

    Camera mainCamera;
    bool showCamera = true;

    bool showProjection = true;
    bool showRendering = true;
    bool showStack = true;
    bool showEnvironment = true;
    bool showOutput = true;


    [MenuItem("Tools/Rendering Control Panel")]
    public static void ShowWindow()
    {
        GetWindow<RenderingControlWindow>("Rendering Control");
    }

    private void OnEnable()
    {
        ambientColor = RenderSettings.ambientLight;
        mainLight = FindDirectionalLight();

        mainCamera = Camera.main;
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Rendering & Lighting Control", EditorStyles.boldLabel);

        showFocusedSettings = EditorGUILayout.Toggle("Focus Settings", showFocusedSettings);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // Línea de separación

        DrawLightingControls();
        EditorGUILayout.Space(10);
        DrawSkyboxControls();

        EditorGUILayout.Space(10);
        DrawCameraControls();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refrescar datos"))
        {
            mainLight = FindDirectionalLight();
            ambientColor = RenderSettings.ambientLight;
        }
    }

    void DrawLightingControls()
    {
        showLighting = EditorGUILayout.Foldout(showLighting, "Iluminación Global", true);
        if (!showLighting) return;

        EditorGUI.indentLevel++;

        // Color de luz ambiental
        ambientColor = EditorGUILayout.ColorField("Ambient Light", ambientColor);
        RenderSettings.ambientLight = ambientColor;

        // Luz direccional principal
        if (mainLight != null)
        {
            mainLight.color = EditorGUILayout.ColorField("Main Light Color", mainLight.color);
            mainLight.intensity = EditorGUILayout.Slider("Intensidad", mainLight.intensity, 0f, 8f);
            mainLight.transform.rotation = Quaternion.Euler(
                EditorGUILayout.Vector3Field("Rotación", mainLight.transform.rotation.eulerAngles)
            );
        }
        else
        {
            EditorGUILayout.HelpBox("No se ha encontrado una luz direccional en la escena.", MessageType.Warning);
        }

        EditorGUI.indentLevel--;
    }

    void DrawSkyboxControls()
    {
        showSkybox = EditorGUILayout.Foldout(showSkybox, "Skybox y Ambient", true);
        if (!showSkybox) return;

        EditorGUI.indentLevel++;

        // Mostrar y editar el material de Skybox
        RenderSettings.skybox = (Material)EditorGUILayout.ObjectField("Skybox Material", RenderSettings.skybox, typeof(Material), false);

        // Espaciado visual
        GUILayout.Space(5);

        EditorGUI.indentLevel--;
    }

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

        #region  Rendering
        EditorGUILayout.BeginVertical("box"); // <-- Empieza el marco
        showRendering = EditorGUILayout.Foldout(showRendering, "Rendering", true);
        EditorGUILayout.EndVertical(); // <-- Termina el marco

        #endregion
        
        #region  Stack
        EditorGUILayout.BeginVertical("box"); // <-- Empieza el marco
        showStack = EditorGUILayout.Foldout(showStack, "Stack", true);
        EditorGUILayout.EndVertical(); // <-- Termina el marco

        #endregion

        #region  Environment
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
        #endregion

        #region  Output
        EditorGUILayout.BeginVertical("box"); // <-- Empieza el marco
        showOutput = EditorGUILayout.Foldout(showOutput, "Output", true);
        EditorGUILayout.EndVertical(); // <-- Termina el marco

        #endregion

        EditorGUI.indentLevel--;
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
}
