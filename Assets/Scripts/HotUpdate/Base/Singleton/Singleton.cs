namespace EggCard
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T m_Ins = null;
        public static T Ins => m_Ins ??= new T();

        protected Singleton()
        {
            
        }
    }
}