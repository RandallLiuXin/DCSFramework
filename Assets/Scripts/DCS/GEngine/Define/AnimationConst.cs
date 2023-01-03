using System;
using System.Collections;
using System.Collections.Generic;

namespace Galaxy.Visual
{
    public enum AnimatorParameterValueType
    {
        AP_Default,
        AP_Bool,
        AP_Int,
        AP_Float,
        AP_Trigger,
    }

    public class SetAnimatorParameterData : IReference
    {
        public string ParameterName;
        public AnimatorParameterValueType ParameterValueType;
        public bool bValue;
        public int iValue;
        public float fValue;

        public void Clear()
        {
            ParameterValueType = AnimatorParameterValueType.AP_Default;
            ParameterName = null;
            bValue = false;
            iValue = 0;
            fValue = 0.0f;
        }
    }

    public class AnimationConst
    {
        public static string AC_Moving = "Moving";
        public static string AC_Trigger = "Trigger";
        public static string AC_TriggerNumber = "TriggerNumber";
        public static string AC_Action = "Action";
        public static string AC_Jumping = "Jumping";
        public static string AC_Idle = "Idle";
        public static string AC_VelocityX = "Velocity X";
        public static string AC_VelocityZ = "Velocity Z";
    }

    public enum AnimatorType
    {
        Player,
    }

    public enum AnimationParameter
    {
        AP_Moving,
    }

    public enum AnimationTriggerType
    {
        Attack = 4,
        Damage = 12,
        Jump = 18,
        KnockBack = 26,
        Knockdown = 27,
        Dodge = 28,
    }

    public enum AnimationJumpState
    {
        JumpingLand = 0,
        JumpingStart = 1,
        JumpingFall = 2,
        JumpingFlip = 3,
    }

    public enum AnimationActionDamage
    {
        ActionDamage_F1 = 1,
        ActionDamage_F2 = 2,
        ActionDamage_B1 = 3,
        ActionDamage_L1 = 4,
        ActionDamage_R1 = 5,
    }

    public enum AnimationActionKnockback
    {
        ActionKnockback_Back1 = 1,
        ActionKnockback_Back2 = 2,
    }

    public enum AnimationActionAttack
    {
        ActionAttack_L1 = 1,
        ActionAttack_L2 = 2,
        ActionAttack_L3 = 3,
        ActionAttack_R1 = 4,
        ActionAttack_R2 = 5,
        ActionAttack_R3 = 6,
    }

    [Flags]
    public enum AnimationEventCallback
    {
        AECB_Default,
        AECB_Hit,
        AECB_FootL,
        AECB_FootR,
        AECB_Land,
    }
}
