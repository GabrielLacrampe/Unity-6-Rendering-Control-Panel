using UnityEditor;
using UnityEngine;
using System.Linq;

public class RenderingControlWindow : EditorWindow
{
    bool showLighting = true;
    bool showSkybox = true;

    Color ambientColor;
    Light mainLight;

    [MenuItem("Tools/Rendering Control Panel")]
    public static void ShowWindow()
    {
        GetWindow<RenderingControlWindow>("Rendering Control");
    }

    private void OnEnable()
    {
        ambientColor = RenderSettings.ambientLight;
        mainLight = FindDirectionalLight();
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Rendering & Lighting Control", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); // Línea de separación

        DrawLightingControls();
        EditorGUILayout.Space(10);
        DrawSkyboxControls();

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
}
