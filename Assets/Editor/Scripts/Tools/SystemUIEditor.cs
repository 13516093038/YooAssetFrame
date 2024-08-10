using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Scripts.Tools
{
    public class SystemUIEditor :UnityEditor.Editor
    {
        [InitializeOnLoadMethod]
        private static void InitEditor()
        {
            //监听hierarchy发生改变的委托
            EditorApplication.hierarchyChanged += HandelTextOrImageRayCast;
        }

        private static void HandelTextOrImageRayCast()
        {
            GameObject obj = Selection.activeGameObject;
            if (obj != null)
            {
                obj.TryGetComponent<Text>(out var text);
                if (text != null)
                {
                    text.raycastTarget = false;
                }
                
                obj.TryGetComponent<Image>(out var image);
                obj.TryGetComponent<Button>(out var button);
                obj.TryGetComponent<Toggle>(out var toggle);
                if (image != null && (button == null || toggle == null))
                {
                    image.raycastTarget = false;
                }
                
                obj.TryGetComponent<RawImage>(out var rawImage);
                if (rawImage != null)
                {
                    rawImage.raycastTarget = false;
                }
            }
        }
    }
}