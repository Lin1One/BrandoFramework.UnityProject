using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Anonym.Util
{
    // Different mechanisms are required in the Editor and Runtime.
    [System.Serializable]
#if UNITY_EDITOR
    public static class LateUpdateUtil
#else
    public class LateUpdateUtil : Singleton<LateUpdateUtil>
#endif
    {
        [SerializeField]
        private static event Action Updates;

        [SerializeField]
        private static event Action WaitingQueue;


        public static void Register(Action method)
        {
#if UNITY_EDITOR
            if (Updates == null)
            {
                EditorApplication.update += update;
            }
#else
            if (IsNull)
                CreateInstance();
#endif
            Updates += method;
        }

        public static void Register_NextUpdate(Action method)
        {
            Register(() => WaitingQueue += method);
        }

#if !UNITY_EDITOR
        private void Update()
        {
            update();
        }
#endif

        private static void update()
        {
            if (Updates != null)
            {
                Updates();
                Updates = null;
            }

            if (WaitingQueue != null)
            {
                Updates += WaitingQueue;
                WaitingQueue = null;
            }

            EmptyCheck();
        }

        private static void EmptyCheck()
        {
            if (Updates == null && WaitingQueue == null)
            {
#if UNITY_EDITOR
                EditorApplication.update -= update;
#else
                DestroySingleton(instance);
#endif
            }
        }
    }
}