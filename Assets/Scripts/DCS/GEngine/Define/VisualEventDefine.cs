using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Galaxy.Event;

namespace Galaxy.Visual.Command
{
    #region Visual

    public class VisualCommandPosition : VisualCommandArgs
    {
        public static readonly int EventId = typeof(VisualCommandPosition).GetHashCode();
        public override int Id => EventId;
        public override VisualCommandType CommandType => VisualCommandType.Position;

        public Vector3 Position;

        public override void Clear()
        {
            Position = Vector3.zero;
        }
    }

    public class VisualCommandRotation : VisualCommandArgs
    {
        public static readonly int EventId = typeof(VisualCommandRotation).GetHashCode();
        public override int Id => EventId;
        public override VisualCommandType CommandType => VisualCommandType.Rotation;

        public Quaternion Rotation;

        public override void Clear()
        {
            Rotation = Quaternion.identity;
        }
    }

    public class VisualCommandScale : VisualCommandArgs
    {
        public static readonly int EventId = typeof(VisualCommandScale).GetHashCode();
        public override int Id => EventId;
        public override VisualCommandType CommandType => VisualCommandType.Scale;

        public Vector3 Scale;

        public override void Clear()
        {
            Scale = Vector3.zero;
        }
    }

    public class VisualQueryPosition : VisualQueryArgs
    {
        public static readonly int EventId = typeof(VisualQueryPosition).GetHashCode();
        public override int Id => EventId;
        public override VisualQueryType QueryType => VisualQueryType.Position;

        public override void Clear()
        {
        }
    }

    public class VisualQueryRotation : VisualQueryArgs
    {
        public static readonly int EventId = typeof(VisualQueryRotation).GetHashCode();
        public override int Id => EventId;
        public override VisualQueryType QueryType => VisualQueryType.Rotation;

        public override void Clear()
        {
        }
    }

    public class VisualQueryGetSkeleton : VisualQueryArgs
    {
        public static readonly int EventId = typeof(VisualQueryGetSkeleton).GetHashCode();
        public override int Id => EventId;
        public override VisualQueryType QueryType => VisualQueryType.GetSkeleton;

        public override void Clear()
        {
        }
    }

    #endregion

    #region ModelProxy

    public class ModelCommandSetAnimatorParameter : VisualCommandArgs
    {
        public static readonly int EventId = typeof(ModelCommandSetAnimatorParameter).GetHashCode();
        public override int Id => EventId;
        public override VisualCommandType CommandType => VisualCommandType.SetAnimatorParameter;

        public List<SetAnimatorParameterData> Datas = new List<SetAnimatorParameterData>();

        public override void Clear()
        {
            foreach (var item in Datas)
            {
                item.Clear();
            }
            Datas.Clear();
        }
    }

    public class ModelCommandSetAnimatorEventCallback : VisualCommandArgs
    {
        public static readonly int EventId = typeof(ModelCommandSetAnimatorEventCallback).GetHashCode();
        public override int Id => EventId;
        public override VisualCommandType CommandType => VisualCommandType.SetAnimatorEventCallback;

        public AnimationEventCallback EventType;
        public UnityAction CallBackAction;

        public override void Clear()
        {
            EventType = AnimationEventCallback.AECB_Default;
            CallBackAction = null;
        }
    }

    public class ModelCommandSetAnimatorRootMotionCallback : VisualCommandArgs
    {
        public static readonly int EventId = typeof(ModelCommandSetAnimatorRootMotionCallback).GetHashCode();
        public override int Id => EventId;
        public override VisualCommandType CommandType => VisualCommandType.SetAnimatorRootMotionCallback;

        public UnityAction<Vector3, Quaternion> CallBackAction;

        public override void Clear()
        {
            CallBackAction = null;
        }
    }

    public class ModelCommandToggleAnimatorRootMotion : VisualCommandArgs
    {
        public static readonly int EventId = typeof(ModelCommandToggleAnimatorRootMotion).GetHashCode();
        public override int Id => EventId;
        public override VisualCommandType CommandType => VisualCommandType.ToggleAnimatorRootMotion;

        public override void Clear()
        {
        }
    }

    public class ModelCommandToggleAnimator : VisualCommandArgs
    {
        public static readonly int EventId = typeof(ModelCommandToggleAnimator).GetHashCode();
        public override int Id => EventId;
        public override VisualCommandType CommandType => VisualCommandType.ToggleAnimator;

        public override void Clear()
        {
        }
    }

    #endregion

    #region Engine

    public class EngineCommandLoadScene : EngineCommandArgs
    {
        public static readonly int EventId = typeof(EngineCommandLoadScene).GetHashCode();
        public override int Id => EventId;
        public override EngineCommandType CommandType => EngineCommandType.LoadScene;

        public string SceneName;

        public override void Clear()
        {
            SceneName = "";
        }
    }

    public class EngineCommandUpdateCameraTargetPos : EngineCommandArgs
    {
        public static readonly int EventId = typeof(EngineCommandUpdateCameraTargetPos).GetHashCode();
        public override int Id => EventId;
        public override EngineCommandType CommandType => EngineCommandType.UpdateCameraTargetPos;

        public Vector3 CameraTargetPos;

        public override void Clear()
        {
            CameraTargetPos = Vector3.zero;
        }
    }

    #endregion
}