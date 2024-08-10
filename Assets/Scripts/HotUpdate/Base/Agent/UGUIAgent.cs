using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public static class UGUIAgent
    {
        public static void SetVisible(this GameObject obj, bool visible)
        {
            obj.transform.localScale = visible ? Vector3.one : Vector3.zero;
        }
        
        public static void SetVisible(this Transform trans, bool visible)
        {
            trans.localScale = visible ? Vector3.one : Vector3.zero;
        }
        
        public static void SetVisible(this Button button, bool visible)
        {
            button.transform.localScale = visible ? Vector3.one : Vector3.zero;
        }
        
        public static void SetVisible(this Image image, bool visible)
        {
            image.transform.localScale = visible ? Vector3.one : Vector3.zero;
        }
        
        public static void SetVisible(this Text text, bool visible)
        {
            text.transform.localScale = visible ? Vector3.one : Vector3.zero;
        }
        
        public static void SetVisible(this Slider slider, bool visible)
        {
            slider.transform.localScale = visible ? Vector3.one : Vector3.zero;
        }
    }
}