using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RenderingControlPanel
{
    public class MainLightSection : EditorSection
    {
        Light main_Light;
        public override void Draw(GUIStyle boldLabel, bool showFocusedSettings)
        {
            main_Light = FindDirectionalLight();
            DrawMainLight_Window();

            //DrawOtherLights_Window();

            //TODO: componente de main light Lens Flare (SRP)
        }

        bool showMainLight_Window = false;
        bool showOtherLights_Window = false;

        bool showMainLight_General = false;
        bool showMainLight_Emission = false;
        bool showMainLight_Rendering = false;
        bool showMainLight_Shadows = false;
        void DrawMainLight_Window()
        {
            EditorGUILayout.BeginVertical("box");
            showMainLight_Window = EditorGUILayout.Foldout(showMainLight_Window, "Main Light", true);
            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel++;

            if (main_Light != null)
            {
                if (showMainLight_Window)
                {
                    Light_General(main_Light);
                    Light_Emission(main_Light);
                    Light_Rendering(main_Light);
                    Light_Shadows(main_Light);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No se ha encontrado una luz direccional en la escena.", MessageType.Warning);
            }


            EditorGUI.indentLevel--;
        }

        void Light_General(Light light)
        {
            EditorGUILayout.BeginVertical("box");
            showMainLight_General = EditorGUILayout.Foldout(showMainLight_General, "General", true);
            EditorGUILayout.EndVertical();
            if (showMainLight_General)
            {
                EditorGUI.indentLevel++;

                light.type = (LightType)EditorGUILayout.EnumPopup("Tipo", light.type);
                light.lightmapBakeType = (LightmapBakeType)EditorGUILayout.EnumPopup("Mode", light.lightmapBakeType);

                EditorGUI.indentLevel--;
            }
        }

        void Light_Emission(Light light)
        {
            EditorGUILayout.BeginVertical("box");
            showMainLight_Emission = EditorGUILayout.Foldout(showMainLight_Emission, "Emission", true);
            EditorGUILayout.EndVertical();
            if (showMainLight_Emission)
            {
                EditorGUI.indentLevel++;

                //TODO falta Light Appearance
                light.color = EditorGUILayout.ColorField("Color", light.color);
                light.intensity = EditorGUILayout.FloatField("Intensidad", light.intensity);
                light.bounceIntensity = EditorGUILayout.FloatField("indirect Multiplier", light.bounceIntensity);
                light.cookie = (Texture)EditorGUILayout.ObjectField("Cookie", light.cookie, typeof(Texture), false);

                EditorGUI.indentLevel--;
            }
        }

        void Light_Rendering(Light light)
        {
            EditorGUILayout.BeginVertical("box");
            showMainLight_Rendering = EditorGUILayout.Foldout(showMainLight_Rendering, "Rendering", true);
            EditorGUILayout.EndVertical();
            if (showMainLight_Rendering)
            {
                EditorGUI.indentLevel++;

                //TODO: falta Rendering Layers
                //light.cullingMask = LayerMaskField("Culling Mask", light.cullingMask);

                EditorGUI.indentLevel--;
            }
        }

        void Light_Shadows(Light light)
        {
            EditorGUILayout.BeginVertical("box");
            showMainLight_Shadows = EditorGUILayout.Foldout(showMainLight_Shadows, "Shadows", true);
            EditorGUILayout.EndVertical();
            if (showMainLight_Shadows)
            {
                EditorGUI.indentLevel++;

                light.shadows = (LightShadows)EditorGUILayout.EnumPopup("Shadow Type", light.shadows);

                EditorGUI.indentLevel++;
                light.shadowAngle = EditorGUILayout.Slider("Baked Shadow Angle", light.shadowAngle, 0f, 90f);

                EditorGUILayout.LabelField("Realtime Shadows", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                light.shadowStrength = EditorGUILayout.Slider("Strenght", light.shadowStrength, 0f, 1f);
                // TODO: falta shadow bias
                light.shadowNearPlane = EditorGUILayout.Slider("Shadow Near Plane", light.shadowNearPlane, 0f, 10f);
                //TODO: falta soft shadow quality
                EditorGUI.indentLevel--;

                //TODO: falta custom shadow layers
                EditorGUI.indentLevel--;

                EditorGUI.indentLevel--;
            }
        }

        void DrawOtherLights_Window()
        {
            var allLights =  GameObject.FindObjectsOfType<Light>().Where(l => l != main_Light).ToArray();
            if (allLights.Length > 0)
            {
                EditorGUILayout.BeginVertical("box");
                showOtherLights_Window = EditorGUILayout.Foldout(showOtherLights_Window, "Other Lights", true);
                EditorGUILayout.EndVertical();

                if (showOtherLights_Window)
                {
                    EditorGUI.indentLevel++;
                    foreach (var light in allLights)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.LabelField(light.name, EditorStyles.miniBoldLabel);
                        Light_General(light);
                        Light_Emission(light);
                        Light_Rendering(light);
                        Light_Shadows(light);

                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space(5);
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        Light FindDirectionalLight()
        {
            return GameObject.FindObjectsOfType<Light>()
                .FirstOrDefault(l => l.type == LightType.Directional);
        }
    }
}
