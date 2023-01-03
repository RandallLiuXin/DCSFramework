using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Galaxy.Visual.Command;

namespace Galaxy.Visual
{
    public sealed class EngineProxy
    {
        private List<GalaxyAction<EngineProxy>> m_FlushHandlers;

        private Vector3 m_CameraTargetPos;
        private static uint m_InterpolationFrameCount = 3;

        // flush args
        private string m_NextSceneName;

        public EngineProxy()
        {
            m_FlushHandlers = new List<GalaxyAction<EngineProxy>>();

            m_NextSceneName = null;
        }

        public void Init()
        {
            GalaxyEntry.GetModule<Dots.EcsAdapterModule>().AddEcsQuery(Dots.EcsQueryType.EQ_PlayerTranslation);
        }

        public void Destroy()
        {
            GalaxyEntry.GetModule<Dots.EcsAdapterModule>().RemoveEcsQuery(Dots.EcsQueryType.EQ_PlayerTranslation);
            m_FlushHandlers.Clear();
        }

        public void Update(float elapseSeconds)
        {
            Flush();
        }

        public void PostUpdate(float elapseSeconds)
        {
            Flush();
        }

        public void CameraUpdate(float elapseSeconds)
        {
            Transform cameraJointTF = Camera.main.transform.parent;
            Vector3 deltaPos = m_CameraTargetPos - cameraJointTF.position;
            cameraJointTF.position += deltaPos / m_InterpolationFrameCount;
        }

        public void CameraPostUpdate(float elapseSeconds)
        {
        }

        public void Flush()
        {
            while (m_FlushHandlers.Count != 0)
            {
                var handler = m_FlushHandlers[0];
                handler(this);
                m_FlushHandlers.RemoveAt(0);
            }
        }

        public void AddFlushHandler(EngineFlushType flushType)
        {
            GalaxyAction<EngineProxy> handler = GalaxyEntry.GetModule<VisualProxyRegister>().GetFlushAction(flushType);
            Debug.Assert(handler != null);
            if (!m_FlushHandlers.Contains(handler))
                m_FlushHandlers.Add(handler);
        }

        //command function
        public static void LoadScene(EngineProxy engine, Event.EngineCommandArgs engineCommand)
        {
            EngineCommandLoadScene cmd = engineCommand as EngineCommandLoadScene;
            Debug.Assert(cmd != null);
            engine.m_NextSceneName = cmd.SceneName;
        }

        public static void UpdateCameraTargetPos(EngineProxy engine, Event.EngineCommandArgs engineCommand)
        {
            EngineCommandUpdateCameraTargetPos cmd = engineCommand as EngineCommandUpdateCameraTargetPos;
            Debug.Assert(cmd != null);
            engine.m_CameraTargetPos = cmd.CameraTargetPos;
        }

        //flush function
        public static void ActiveScene(EngineProxy engine)
        {
            SceneManager.LoadScene(engine.m_NextSceneName);
            engine.m_NextSceneName = "";
        }

        //get function
        public string GetScene()
        {
            return SceneManager.GetActiveScene().name;
        }

        public Camera GetMainCamera()
        {
            return Camera.main;
        }
    }
}
