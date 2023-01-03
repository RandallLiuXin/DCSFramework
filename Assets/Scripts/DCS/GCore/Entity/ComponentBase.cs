using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Galaxy.Entities
{
    public enum AccessType
    {
        ReadWrite,
        ReadOnly,
    }

    /// <summary>
    /// 框架数据基类，所有的数据都是在component中声明的
    /// 这一层目的是方便策划快速开发符合直觉的功能。如蓝图、技能、属性等。大多为事件触发非持续性行为
    /// </summary>
    [Serializable]
    public abstract class ComponentBase
    {
        /// <summary>
        /// 返回自己定义在ComponentDefine中的枚举
        /// </summary>
        /// <returns></returns>
        public abstract CompType GetCompType();
    }

    public abstract class ComponentProxy
    {
        protected AccessType m_AccessType;
        public ComponentProxy(ComponentBase data, AccessType accessType)
        {
            Debug.Assert(data != null);
            m_AccessType = accessType;
        }
        public AccessType GetAccessType()
        {
            return m_AccessType;
        }
        public abstract ComponentBase GetComponentBase();
    }
}
