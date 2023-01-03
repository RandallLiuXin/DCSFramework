using System;

namespace Galaxy.Visual
{
    public enum VisualType
    {
        Model,
        Count,
    }

    public enum VisualCommandType
    {
        Position,
        Rotation,
        Scale,

        SetAnimatorParameter,
        SetAnimatorEventCallback,
        SetAnimatorRootMotionCallback,
        ToggleAnimatorRootMotion,
        ToggleAnimator,
        Count,
    }

    public enum VisualFlushType
    {
        Position,
        Rotation,
        Scale,

        SetAnimatorParameter,
        SetAnimatorEventCallback,
        SetAnimatorRootMotionCallback,
        ToggleAnimatorRootMotion,
        ToggleAnimator,
        Count,
    }

    public enum EngineCommandType
    {
        LoadScene,
        UpdateCameraTargetPos,
        Count,
    }

    public enum EngineFlushType
    {
        ActiveScene,
        Count,
    }

    public enum VisualQueryType
    {
        Position,
        Rotation,
        GetSkeleton,
        Count,
    }

    public enum EngineQueryType
    {
        GetScene,
        Count,
    }

    public class VisualDefine
    {
        public static Type[] VisualTypes = new Type[(int)VisualType.Count]
        {
            typeof(ModelProxy),
        };
    }
}
