using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Galaxy;
using UnityEngine.EventSystems;

namespace Anvil.Input
{
    public class InputConst
    {
        public static string InputDefaultMap = "Normal";
        public static string UIMap = "UI";
    }

    /// <summary>
    /// ����������������ע��
    /// 
    /// ����inputsystem֧�ֽ�UI����ת��ΪĳЩkeycode/gamepadӳ��
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class InputProxy : MonoBehaviour
    {
        [HideInInspector]
        public Vector2 InputVector;
        [HideInInspector]
        public bool Attack;

        private void Start()
        {
            InputVector = Vector2.zero;
            Attack = false;

            InputProxyInstance.SetInstance(this);
        }

        public void LocomotionInput(InputAction.CallbackContext context)
        {
            Vector2 inputVector = context.ReadValue<Vector2>();
            InputVector = inputVector;
        }

        public void AttackInput(InputAction.CallbackContext context)
        {
            //if (EventSystem.current.IsPointerOverGameObject())
            //{
            //    return;
            //}

            if (context.phase == InputActionPhase.Canceled)
            {
                Attack = true;
            }
        }

        /// <summary>
        /// ��ͬ����ģʽ�л�
        /// </summary>
        /// <param name="actionMapName"></param>
        public void ChangeInputActionMap(string actionMapName)
        {
            Debug.Log("ChangeInputActionMap name: " + actionMapName);
            Debug.Assert(!actionMapName.IsNE());
            GetComponent<UnityEngine.InputSystem.PlayerInput>().SwitchCurrentActionMap(actionMapName);
        }
    }

    public class InputProxyInstance
    {
        public static InputProxy Instance
        {
            get
            {
                Debug.Assert(m_Instance != null);
                return m_Instance;
            }
        }

        private static InputProxy m_Instance;

        public static void SetInstance(InputProxy proxy)
        {
            Debug.Assert(m_Instance == null);
            m_Instance = proxy;
        }
    }
}
