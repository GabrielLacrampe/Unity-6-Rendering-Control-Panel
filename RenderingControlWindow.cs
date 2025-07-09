using UnityEditor;
using UnityEngine;
using System.Linq;

public class RenderingControlWindow : EditorWindow
{
    bool showLighting = true;
    bool showSkybox = true;


    Color ambientColor;
    Light mainLight;

    Camera mainCamera;
    bool showCamera = true;

    bool showProjection = true;
    bool showEnvironment = true;


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


            // Field of View Axis
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
            mainCamera.nearClipPlane = EditorGUILayout.FloatField("Near", mainCamera.nearClipPlane);
            mainCamera.farClipPlane = EditorGUILayout.FloatField("Far", mainCamera.farClipPlane);

#if UNITY_2020_1_OR_NEWER
            // Physical Camera (solo en Perspective)
            mainCamera.usePhysicalProperties = !mainCamera.orthographic && EditorGUILayout.Toggle("Physical Camera", mainCamera.usePhysicalProperties);

            if (mainCamera.usePhysicalProperties && !mainCamera.orthographic)
            {
                mainCamera.focalLength = EditorGUILayout.FloatField("Focal Length", mainCamera.focalLength);
                mainCamera.sensorSize = EditorGUILayout.Vector2Field("Sensor Size", mainCamera.sensorSize);
                mainCamera.lensShift = EditorGUILayout.Vector2Field("Lens Shift", mainCamera.lensShift);
            }
#endif
            EditorGUI.indentLevel--;
        }
        #endregion

        #region  Environment
        EditorGUILayout.BeginVertical("box"); // <-- Empieza el marco
        showEnvironment = EditorGUILayout.Foldout(showEnvironment, "Environment", true);
        EditorGUILayout.EndVertical(); // <-- Termina el marco

        if (showEnvironment)
        {
            // Clear Flags
            mainCamera.clearFlags = (CameraClearFlags)EditorGUILayout.EnumPopup("Clear Flags", mainCamera.clearFlags);

            // Background color
            if (mainCamera.clearFlags == CameraClearFlags.SolidColor)
            {
                mainCamera.backgroundColor = EditorGUILayout.ColorField("Background Color", mainCamera.backgroundColor);
            }
        }
        #endregion


        EditorGUI.indentLevel--;
    }


}
