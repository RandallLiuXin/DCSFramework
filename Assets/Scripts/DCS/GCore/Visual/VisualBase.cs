using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Timer;
using Galaxy.Visual.Command;

namespace Galaxy.Visual
{
    public abstract class VisualBase
    {
        protected enum VisualState
        {
            ST_NotInited,
            ST_Inited,
            ST_Loading,
            ST_Loaded,
            ST_Attached,
            ST_Destoryed,
        }

        protected VisualState m_State;
        protected uint m_Pid;
        protected uint m_Vid;
        // TODO Randall 这里怎么更适配？使用泛型？
        protected GameObject m_ResObject;
        protected string m_ResPath;

        protected Coroutine m_ResAsyncTask;
        // https://stackoverflow.com/questions/4317479/func-vs-action-vs-predicate
        protected List<GalaxyAction<VisualBase>> m_FlushHandlers;

        //command
        public Vector3 m_Position;
        public Quaternion m_Rotation;
        public Vector3 m_Scale;

        public VisualBase(uint pid, uint vid, string resPath)
        {
            m_State = VisualState.ST_NotInited;
            m_Pid = pid;
            m_Vid = vid;
            m_ResObject = null;
            m_ResPath = resPath;

            m_ResAsyncTask = null;
            m_FlushHandlers = new List<GalaxyAction<VisualBase>>();

            m_Position = new Vector3();
            m_Rotation = new Quaternion();
            m_Scale = new Vector3();
        }

        public void Init(params object[] initArgs)
        {
            InitInternal(initArgs);
            m_State = VisualState.ST_Inited;
            m_ResAsyncTask = RoutineRunner.Instance.StartCoroutine(LoadRes());
        }

        protected virtual void InitInternal(params object[] initArgs)
        {

        }

        public void Destroy()
        {
            //TODO Randall 添加owner和timer id关系
            //TimerManager.RemoveTimer(this, )
            CancelResTask();

            UnityEngine.Object.Destroy(m_ResObject);
            m_FlushHandlers.Clear();
            m_FlushHandlers = null;
            m_State = VisualState.ST_Destoryed;
        }

        public virtual void Update(float elapseSeconds)
        {
            Flush();
        }

        public virtual void PostUpdate(float elapseSeconds)
        {
            Flush();
        }

        public virtual void Flush()
        {
            if (!IsAttached())
                return;
            // 可能在flush过程再次添加新的flushType，因此这里使用while
            while (m_FlushHandlers.Count != 0)
            {
                var handler = m_FlushHandlers[0];
                handler(this);
                m_FlushHandlers.RemoveAt(0);
            }
        }

        public void AddFlushHandler(VisualFlushType flushType)
        {
            GalaxyAction<VisualBase> handler = GalaxyEntry.GetModule<VisualProxyRegister>().GetFlushAction(flushType);
            Debug.Assert(handler != null);
            if (!m_FlushHandlers.Contains(handler))
                m_FlushHandlers.Add(handler);
        }

        protected IEnumerator LoadRes()
        {
            Debug.Assert(IsInited());
            Debug.Assert(!m_ResPath.IsNE());
            ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(m_ResPath);
            m_State = VisualState.ST_Loading;
            resourceRequest.completed += ResTaskComplete;
            yield return resourceRequest;
        }

        protected void ResTaskComplete(AsyncOperation operation)
        {
            m_ResAsyncTask = null;
            var resourceRequest = operation as ResourceRequest;
            Debug.Assert(resourceRequest.isDone);
            Debug.Assert(resourceRequest.asset != null);
            m_State = VisualState.ST_Loaded;
            ResAsyncLoad(resourceRequest.asset);

            //Mono
            Debug.Assert(m_ResObject != null);
            MonoInit();
        }

        protected virtual VisualMonoBase MonoInit()
        {
            var monoBase = m_ResObject.AddComponent<VisualMonoBase>();
            monoBase.VisualPid = m_Pid;
            monoBase.VisualVid = m_Vid;
            return monoBase;
        }

        protected VisualMonoBase GetVisualMonoBase()
        {
            return m_ResObject.GetComponent<VisualMonoBase>();
        }

        protected abstract void ResAsyncLoad(UnityEngine.Object asset); //m_ResObject = asset as GameObject;

        protected void CancelResTask()
        {
            if (m_ResAsyncTask == null)
                return;
            RoutineRunner.Instance.StopCoroutine(m_ResAsyncTask);
        }

        public void AttachFinish()
        {
            m_State = VisualState.ST_Attached;
            Flush();
        }

        public void AttachToScene(GameObject logicObject)
        {
            Debug.Assert(IsLoaded());
            Debug.Assert(m_ResObject != null);
            m_ResObject.SetActive(true);

            m_ResObject.transform.SetParent(logicObject.transform);
        }

        public void RemoveFromParent()
        {
            if (!IsAttached())
                return;
            Debug.Assert(m_ResObject != null);
            m_ResObject.transform.SetParent(null);
            m_ResObject.SetActive(false);
            m_State = VisualState.ST_Loaded;
        }

        public void BindToBone(GameObject resObj, object[] args)
        {
            Debug.Assert(args.Length == 4);
            string bindTargetName = args[0] as string;
            Debug.Assert(bindTargetName != null);
            Vector3 localPosition = (Vector3)args[1];
            Quaternion localRotaion = (Quaternion)args[2];
            Vector3 localScale = (Vector3)args[3];

            Transform boneTF = m_ResObject.transform.Find(bindTargetName);
            Debug.Assert(boneTF != null);
            resObj.transform.SetParent(boneTF);
            resObj.transform.localPosition = localPosition;
            resObj.transform.localRotation = localRotaion;
            resObj.transform.localScale = localScale;
            resObj.SetActive(true);
        }

        public uint VID => m_Vid;
        public GameObject ResObject => m_ResObject;
        public bool IsNotInited() { return m_State == VisualState.ST_NotInited; }
        public bool IsInited() { return m_State == VisualState.ST_Inited; }
        public bool IsLoading() { return m_State == VisualState.ST_Loading; }
        public bool IsLoaded() { return m_State == VisualState.ST_Loaded || m_State == VisualState.ST_Attached; }
        public bool IsAttached() { return m_State == VisualState.ST_Attached; }
        public bool IsDestroyed() { return m_State == VisualState.ST_Destoryed; }

        public abstract VisualType GetVisualType();

        //command function
        public static void UpdatePosition(VisualBase visual, Event.VisualCommandArgs visualCommand)
        {
            VisualCommandPosition cmd = visualCommand as VisualCommandPosition;
            Debug.Assert(cmd != null);
            visual.m_Position = cmd.Position;
        }

        public static void UpdateRotation(VisualBase visual, Event.VisualCommandArgs visualCommand)
        {
            VisualCommandRotation cmd = visualCommand as VisualCommandRotation;
            Debug.Assert(cmd != null);
            visual.m_Rotation = cmd.Rotation;
        }

        public static void UpdateScale(VisualBase visual, Event.VisualCommandArgs visualCommand)
        {
            VisualCommandScale cmd = visualCommand as VisualCommandScale;
            Debug.Assert(cmd != null);
            visual.m_Scale = cmd.Scale;
        }

        //flush function
        public static void FlushPosition(VisualBase visual)
        {
            visual.m_ResObject.transform.localPosition = visual.m_Position;
        }

        public static void FlushRotation(VisualBase visual)
        {
            visual.m_ResObject.transform.localRotation = visual.m_Rotation;
        }

        public static void FlushScale(VisualBase visual)
        {
            visual.m_ResObject.transform.localScale = visual.m_Scale;
        }

        //get function
        public Vector3 GetPosition()
        {
            return m_ResObject.transform.position;
        }

        public Quaternion GetRotation()
        {
            return m_ResObject.transform.rotation;
        }
    }
}
