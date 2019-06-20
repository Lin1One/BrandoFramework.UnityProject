using Client.UI.Collections;
using System.Collections.Generic;

namespace Client.UI
{
    /// <summary>
    /// ²Ã¼ô×¢²á´¦
    /// </summary>
    public class ClipperRegistry
    {
        static ClipperRegistry s_Instance;
        public static ClipperRegistry instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new ClipperRegistry();
                return s_Instance;
            }
        }

        readonly IndexedSet<IClipper> m_Clippers = new IndexedSet<IClipper>();

        protected ClipperRegistry()
        {
            // This is needed for AOT platforms. Without it the compile doesn't get the definition of the Dictionarys
#pragma warning disable 168
            Dictionary<IClipper, int> emptyIClipperDic;
#pragma warning restore 168
        }

        /// <summary>
        /// ÌÞ³ý²Ù×÷
        /// </summary>
        public void Cull()
        {
            for (var i = 0; i < m_Clippers.Count; ++i)
            {
                m_Clippers[i].PerformClipping();
            }
        }

        public static void Register(IClipper c)
        {
            if (c == null)
                return;
            instance.m_Clippers.Add(c);
        }

        public static void Unregister(IClipper c)
        {
            instance.m_Clippers.Remove(c);
        }
    }
}
