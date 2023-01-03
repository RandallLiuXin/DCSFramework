using System;
using System.Collections;
using UnityEngine;

namespace Galaxy.Timer
{
    public class RoutineRunner : MonoBehaviour
    {
        private static RoutineRunner m_Instance = null;
        public static RoutineRunner Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    throw new GalaxyException("We must have a RoutineRunner instance");
                }
                return m_Instance;
            }
        }

        private void Awake()
        {
            Debug.Assert(m_Instance == null);
            m_Instance = this;
        }

        public static T InstantiateGameObject<T>(UnityEngine.Object objectInLibrary) where T : UnityEngine.Object
        {
            UnityEngine.Object obj = Instantiate(objectInLibrary);
            T t = obj as T;
            if (obj != null && t == null)
            {
                Debug.LogError("Trying to create an object named " + objectInLibrary.name + ", but expected the wrong type.");
            }
            return t;
        }
        public static GameObject InstantiateGameObject(string name, Transform parent = null)
        {
            GameObject gameObject = null;
            try
            {
                gameObject = (Instantiate(Resources.Load(name, typeof(GameObject))) as GameObject);
                if (parent != null)
                {
                    gameObject.transform.SetParent(parent, true);
                }
            }
            catch
            {
                Debug.LogError("InstantiateGameObject failed. Name: " + name);
            }
            return gameObject;
        }
        public static T InstantiateWithComponent<T>(string name, Transform parent = null) where T : Component
        {
            T result = (T)((object)null);
            try
            {
                GameObject gameObject = Instantiate(Resources.Load(name, typeof(GameObject))) as GameObject;
                result = gameObject.GetComponent<T>();
                if (parent != null)
                {
                    gameObject.transform.SetParent(parent, true);
                }
            }
            catch
            {
                Debug.LogError("InstantiateWithComponent failed. Name: " + name);
            }
            return result;
        }


        public static GameObject GetResFormResourcesFold(string strPath, string strRes)
        {
            string str = strPath;
            str += '/';
            str += strRes;
            return Resources.Load<GameObject>(str);
        }

        public static void WaitForSeconds(float fSeconds, Action action)
        {
            Instance.StartCoroutine(Instance.WaitForSecondsCoroutine(fSeconds, action));
        }

        private IEnumerator WaitForSecondsCoroutine(float fSeconds, Action action)
        {
            yield return new WaitForSeconds(fSeconds);
            //yield return Yielders.WaitSecond(fSeconds);
            if (action != null)
            {
                action();
            }
        }

        public static void WaitOneFrame(Action action)
        {
            Instance.StartCoroutine(Instance.WaitFrameCoroutine(action, 1));
        }

        public static void WaitFrame(Action action, int count)
        {
            Instance.StartCoroutine(Instance.WaitFrameCoroutine(action, count));
        }

        private IEnumerator WaitFrameCoroutine(Action action, int count)
        {
            while ((count--) >= 0)
            {
                yield return new WaitForEndOfFrame();
                //yield return Yielders.WaitEndOfFrame;
            }
            if (action != null)
            {
                action();
            }
        }
    }
}