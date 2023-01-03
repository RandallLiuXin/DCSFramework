using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Galaxy.Visual
{
    public class VisualMonoBase : MonoBehaviour
    {
        public uint VisualPid;
        public uint VisualVid;

        //only work for pawn
        public uint HolderUid;

        private void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

#if UNITY_EDITOR
        private GUIStyle EQSGuiStyle = null;
        public bool NeedToDrawEQS = false;
        public float EQSScore = 0.0f;

        private void OnDrawGizmos()
        {
            if (!NeedToDrawEQS)
            {
                return;
            }
            if (EQSGuiStyle == null)
            {
                EQSGuiStyle = new GUIStyle();
                EQSGuiStyle.fontSize = 20;
                EQSGuiStyle.fontStyle = FontStyle.Bold;
            }

            Handles.Label(transform.position, Utility.Text.Format("Score: {0}", EQSScore), EQSGuiStyle);
        }

        public void ShowDebugStr(string debugType, List<object> debugInfos)
        {
            if (debugType == "DrawEQS")
            {
                Debug.Assert(debugInfos.Count == 2);
                NeedToDrawEQS = (bool)debugInfos[0];
                EQSScore = (float)debugInfos[1];
            }
        }
#endif
    }
}
