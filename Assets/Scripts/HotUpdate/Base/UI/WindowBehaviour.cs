using UnityEngine;

namespace HotUpdate
{
    public abstract class WindowBehaviour
    {
        public GameObject gameObject { get; set; }
        public Transform transform { get; set; }
        public Canvas Canvas { get; set; }
        public string Name { get; set; }
        public bool Visible { get; set; }

        public virtual void OnAwake()
        {
            Debug.Log(this.Name + " OnAwake");
        }

        public virtual void OnShow()
        {
            Debug.Log(this.Name + " OnShow");
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnHide()
        {
            Debug.Log(this.Name + " OnHide");
        }

        public virtual void OnDestroy()
        {
            Debug.Log(this.Name + " OnDestroy");
        }

        public virtual void SetVisible(bool isVisible)
        {
        }
    }
}