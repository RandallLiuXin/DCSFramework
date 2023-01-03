using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Galaxy.Visual
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorEvent_Boxer : MonoBehaviour
    {
        // Event call functions for Animation events.
        public UnityEvent OnHit = new UnityEvent();
        public UnityEvent OnFootR = new UnityEvent();
        public UnityEvent OnFootL = new UnityEvent();
        public UnityEvent OnLand = new UnityEvent();

        public UnityEvent<Vector3, Quaternion> OnMove = new UnityEvent<Vector3, Quaternion>();
        private Animator animator;

        private bool m_ApplyRootMotion = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
            ToggleApplyRootMotion();
        }

        public void Hit() => OnHit.Invoke();
        public void FootR() => OnFootR.Invoke();
        public void FootL() => OnFootL.Invoke();
        public void Land() => OnLand.Invoke();

        void OnAnimatorMove()
        {
            if (!animator)
                return;

            if (!animator.applyRootMotion)
                return;

            Vector3 deltaPos = animator.deltaPosition;
            if (Mathf.Abs(deltaPos.x) < 0.01f
                && Mathf.Abs(deltaPos.y) < 0.01f
                && Mathf.Abs(deltaPos.z) < 0.01f)
            {
                return;
            }

            //Debug.Log("AnimatorEvent_Boxer OnAnimatorMove: " + animator.deltaPosition);
            animator.transform.position += animator.deltaPosition;
            OnMove.Invoke(animator.deltaPosition, animator.deltaRotation);
        }

        public void ToggleApplyRootMotion()
        {
            if (animator == null)
                return;
            m_ApplyRootMotion = !m_ApplyRootMotion;
            animator.applyRootMotion = m_ApplyRootMotion;
        }
    }
}
