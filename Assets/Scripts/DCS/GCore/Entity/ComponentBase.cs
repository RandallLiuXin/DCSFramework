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
    /// ������ݻ��࣬���е����ݶ�����component��������
    /// ��һ��Ŀ���Ƿ���߻����ٿ�������ֱ���Ĺ��ܡ�����ͼ�����ܡ����Եȡ����Ϊ�¼������ǳ�������Ϊ
    /// </summary>
    [Serializable]
    public abstract class ComponentBase
    {
        /// <summary>
        /// �����Լ�������ComponentDefine�е�ö��
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
