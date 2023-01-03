using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Common
{
    public class DontDestoryTool : MonoBehaviour
    {
        public bool DontDestoryOnLoad;
        public bool DontCreateNewWhenBackToThisScene;
        public static DontDestoryTool Instance = null;
        void Awake()
        {
            if (!DontCreateNewWhenBackToThisScene)
            {
                return;
            }
            if (Instance != null)
            {
                GameObject.Destroy(this.gameObject);
                return;
            }
            Instance = this;
            if (this.DontDestoryOnLoad)
                GameObject.DontDestroyOnLoad(this);

            DontCreateNewWhenBackToThisScene = false;
        }
    }
}
