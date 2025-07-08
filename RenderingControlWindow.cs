using UnityEditor;
using UnityEngine;

public class RenderingControlWindow : EditorWindow
{
    [MenuItem("Tools/Rendering Control Panel")]
    public static void ShowWindow()
    {
        GetWindow<RenderingControlWindow>("Rendering Control");
    }

    void OnGUI()
    {
        GUILayout.Label("Render & Lighting Settings", EditorStyles.boldLabel);
    }
}
