using Galaxy.Visual.Command;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Galaxy.Visual
{
    public class ModelProxy : VisualBase
    {
        private Animator m_Animator;
        private AnimatorEvent_Boxer m_AnimatorEvent;

        private Dictionary<string, bool> m_BoolParameterDict;
        private Dictionary<string, int> m_IntParameterDict;
        private Dictionary<string, float> m_FloatParameterDict;
        private Dictionary<string, bool> m_TriggerParameterDict;

        private Dictionary<AnimationEventCallback, UnityAction> m_EventCallbacks;
        private UnityAction<Vector3, Quaternion> m_RootMotionCallback;

        private List<Transform> m_Skeleton;

        public ModelProxy(uint pid, uint vid, string resPath) : base(pid, vid, resPath)
        {
            m_BoolParameterDict = new Dictionary<string, bool>();
            m_IntParameterDict = new Dictionary<string, int>();
            m_FloatParameterDict = new Dictionary<string, float>();
            m_TriggerParameterDict = new Dictionary<string, bool>();

            m_EventCallbacks = new Dictionary<AnimationEventCallback, UnityAction>();
            m_RootMotionCallback = null;
        }

        public override VisualType GetVisualType()
        {
            return VisualType.Model;
        }

        protected override void ResAsyncLoad(Object asset)
        {
            GameObject assetObject = asset as GameObject;
            Debug.Assert(assetObject != null);
            m_ResObject = GameObject.Instantiate<GameObject>(assetObject);
            m_ResObject.SetActive(false);

            m_Animator = m_ResObject.GetComponent<Animator>();
            m_AnimatorEvent = m_ResObject.GetComponent<AnimatorEvent_Boxer>();
            InitBoneSkeleton();
        }

        public bool HasAnimator()
        {
            return m_Animator != null && m_AnimatorEvent != null;
        }

        //command function
        public static void SetAnimatorParameter(VisualBase visual, Event.VisualCommandArgs visualCommand)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);
            ModelCommandSetAnimatorParameter cmd = visualCommand as ModelCommandSetAnimatorParameter;
            Debug.Assert(cmd != null);
            foreach (var data in cmd.Datas)
            {
                switch (data.ParameterValueType)
                {
                    case AnimatorParameterValueType.AP_Bool:
                        if (model.m_BoolParameterDict.ContainsKey(data.ParameterName))
                        {
                            model.m_BoolParameterDict[data.ParameterName] = data.bValue;
                        }
                        else
                        {
                            model.m_BoolParameterDict.Add(data.ParameterName, data.bValue);
                        }
                        break;
                    case AnimatorParameterValueType.AP_Int:
                        if (model.m_IntParameterDict.ContainsKey(data.ParameterName))
                        {
                            model.m_IntParameterDict[data.ParameterName] = data.iValue;
                        }
                        else
                        {
                            model.m_IntParameterDict.Add(data.ParameterName, data.iValue);
                        }
                        break;
                    case AnimatorParameterValueType.AP_Float:
                        if (model.m_FloatParameterDict.ContainsKey(data.ParameterName))
                        {
                            model.m_FloatParameterDict[data.ParameterName] = data.fValue;
                        }
                        else
                        {
                            model.m_FloatParameterDict.Add(data.ParameterName, data.fValue);
                        }
                        break;
                    case AnimatorParameterValueType.AP_Trigger:
                        if (model.m_TriggerParameterDict.ContainsKey(data.ParameterName))
                        {
                            model.m_TriggerParameterDict[data.ParameterName] = data.bValue;
                        }
                        else
                        {
                            model.m_TriggerParameterDict.Add(data.ParameterName, data.bValue);
                        }
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
        }

        public static void SetAnimatorEventCallback(VisualBase visual, Event.VisualCommandArgs visualCommand)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);
            ModelCommandSetAnimatorEventCallback cmd = visualCommand as ModelCommandSetAnimatorEventCallback;
            Debug.Assert(cmd != null);
            model.m_EventCallbacks[cmd.EventType] = cmd.CallBackAction;
        }

        public static void SetAnimatorRootMotionCallback(VisualBase visual, Event.VisualCommandArgs visualCommand)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);
            ModelCommandSetAnimatorRootMotionCallback cmd = visualCommand as ModelCommandSetAnimatorRootMotionCallback;
            Debug.Assert(cmd != null);
            model.m_RootMotionCallback = cmd.CallBackAction;
        }

        public static void ToggleAnimatorRootMotion(VisualBase visual, Event.VisualCommandArgs visualCommand)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);
            ModelCommandToggleAnimatorRootMotion cmd = visualCommand as ModelCommandToggleAnimatorRootMotion;
            Debug.Assert(cmd != null);
        }

        public static void ToggleAnimator(VisualBase visual, Event.VisualCommandArgs visualCommand)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);
            ModelCommandToggleAnimator cmd = visualCommand as ModelCommandToggleAnimator;
            Debug.Assert(cmd != null);
        }

        //flush function
        public static void FlushSetAnimatorParameter(VisualBase visual)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);

            foreach (var item in model.m_BoolParameterDict)
            {
                model.m_Animator.SetBool(item.Key, item.Value);
            }
            model.m_BoolParameterDict.Clear();

            foreach (var item in model.m_IntParameterDict)
            {
                model.m_Animator.SetInteger(item.Key, item.Value);
            }
            model.m_IntParameterDict.Clear();

            foreach (var item in model.m_FloatParameterDict)
            {
                model.m_Animator.SetFloat(item.Key, item.Value);
            }
            model.m_FloatParameterDict.Clear();

            foreach (var item in model.m_TriggerParameterDict)
            {
                if (item.Value)
                {
                    model.m_Animator.SetTrigger(item.Key);
                }
                else
                {
                    model.m_Animator.ResetTrigger(item.Key);
                }
            }
            model.m_TriggerParameterDict.Clear();
        }

        public static void FlushSetAnimatorEventCallback(VisualBase visual)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);

            foreach (var item in model.m_EventCallbacks)
            {
                var eventType = item.Key;
                var action = item.Value;
                switch (eventType)
                {
                    case AnimationEventCallback.AECB_Hit:
                        model.m_AnimatorEvent.OnHit.RemoveAllListeners();
                        model.m_AnimatorEvent.OnHit.AddListener(action);
                        break;
                    case AnimationEventCallback.AECB_FootL:
                        model.m_AnimatorEvent.OnFootL.RemoveAllListeners();
                        model.m_AnimatorEvent.OnFootL.AddListener(action);
                        break;
                    case AnimationEventCallback.AECB_FootR:
                        model.m_AnimatorEvent.OnFootR.RemoveAllListeners();
                        model.m_AnimatorEvent.OnFootR.AddListener(action);
                        break;
                    case AnimationEventCallback.AECB_Land:
                        model.m_AnimatorEvent.OnLand.RemoveAllListeners();
                        model.m_AnimatorEvent.OnLand.AddListener(action);
                        break;
                    default:
                        break;
                }
            }
            model.m_EventCallbacks.Clear();
        }

        public static void FlushSetAnimatorRootMotionCallback(VisualBase visual)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);
            Debug.Assert(model.m_RootMotionCallback != null);

            model.m_AnimatorEvent.OnMove.RemoveAllListeners();
            model.m_AnimatorEvent.OnMove.AddListener(model.m_RootMotionCallback);
            model.m_RootMotionCallback = null;
        }

        public static void FlushToggleAnimatorRootMotion(VisualBase visual)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);
            Debug.Assert(model.m_AnimatorEvent != null);

            model.m_AnimatorEvent.ToggleApplyRootMotion();
        }

        public static void FlushToggleAnimator(VisualBase visual)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);

            model.m_Animator.enabled = !model.m_Animator.enabled;
        }

        //get function
        private void InitBoneSkeleton()
        {
            Transform rootTF = m_ResObject.transform.GetChild(1);
#if UNITY_EDITOR
            Debug.Assert(rootTF.name.Equals("Root") && m_Skeleton == null);
#endif
            var results = rootTF.GetComponentsInChildren<Anvil.Animation.SkeletonBoneAuthoring>(true);
            m_Skeleton = new List<Transform>();
            for (int i = 0; i < results.Length; i++)
            {
                m_Skeleton.Add(null);
            }

            foreach (var item in results)
            {
                m_Skeleton[item.BoneIndex] = item.transform;
            }
        }

        public List<Transform> GetSkeleton()
        {
            return m_Skeleton;
        }
    }
}
