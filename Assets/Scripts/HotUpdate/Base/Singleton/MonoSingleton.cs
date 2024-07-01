using System;

namespace EggCard
{
    public abstract class MonoSingleton<T> : UnityEngine.MonoBehaviour where T : UnityEngine.MonoBehaviour
    {
        private static T m_Ins;
        
        public static T Ins
        {
            get
            {
                return m_Ins;
            }
        }

        protected virtual void Awake()
        {
            if (m_Ins == null)
            {
                m_Ins = this as T;
            }
        }
    }
}