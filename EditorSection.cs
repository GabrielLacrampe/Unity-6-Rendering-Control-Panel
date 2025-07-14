using UnityEngine;

namespace RenderingControlPanel
{
    public abstract class EditorSection
    {
        public abstract void Draw(GUIStyle boldLabel, bool showFocusedSettings);
    }
}
