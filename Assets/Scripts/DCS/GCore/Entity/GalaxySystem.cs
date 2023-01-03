using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Galaxy.Data;

namespace Galaxy.Entities
{
    public sealed class SystemTypeDefine : HolderTypeDefine
    {
        public SystemTypeDefine(SystemType systemType, SystemType[] systems, CompType[] rwComps, CompType[] roComps)
            : base(systems, rwComps, roComps)
        {
            m_SystemType = systemType;
        }

        public SystemType m_SystemType;
    }

    /// <summary>
    /// ���ϵͳ����
    /// </summary>
    public abstract class GalaxySystem
    {
        public GalaxySystem()
        {

        }

        /// <summary>
        /// �����Լ�������SystemDefine�е�ö��
        /// </summary>
        /// <returns></returns>
        public SystemType GetSystemType() => GetSystemDefine().m_SystemType;

        /// <summary>
        /// ������������ݶ���
        /// </summary>
        /// <returns></returns>
        public abstract SystemTypeDefine GetSystemDefine();

        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="uid">����id</param>
        internal abstract void Init(HolderProxy holder);

        /// <summary>
        /// ��ʼ���������
        /// </summary>
        /// <param name="uid">����id</param>
        internal abstract void Initialize(HolderProxy holder);

        /// <summary>
        /// ��ѯ��ǰ����
        /// </summary>
        /// <param name="uid">����id</param>
        /// <param name="elapseSeconds">�߼�����ʱ�䣬����Ϊ��λ��</param>
        internal abstract void Update(HolderProxy holder, float elapseSeconds);

        /// <summary>
        /// ��ѯ��ǰ����
        /// </summary>
        /// <param name="uid">����id</param>
        /// <param name="elapseSeconds">�߼�����ʱ�䣬����Ϊ��λ��</param>
        internal abstract void PostUpdate(HolderProxy holder, float elapseSeconds);

        /// <summary>
        /// ��ѯ��ǰ����
        /// </summary>
        /// <param name="uid">����id</param>
        /// <param name="elapseSeconds">�߼�����ʱ�䣬����Ϊ��λ��</param>
        internal abstract void FixedUpdate(HolderProxy holder, float elapseSeconds);

        /// <summary>
        /// �Ƴ�����֮ǰ
        /// </summary>
        /// <param name="uid">����id</param>
        internal abstract void PreDestroy(HolderProxy holder);

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="uid">����id</param>
        internal abstract void Destroy(HolderProxy holder);
    }

    /// <summary>
    /// Oopϵͳ
    /// </summary>
    public abstract class GameSystem : GalaxySystem
    {
        internal override void Initialize(HolderProxy holder) { }
        internal override void PostUpdate(HolderProxy holder, float elapseSeconds) { }
        internal override void FixedUpdate(HolderProxy holder, float elapseSeconds) { }
        internal override void PreDestroy(HolderProxy holder) { }
    }

    public abstract class GalaxySystemProxy
    {
        public GalaxySystemProxy()
        {
        }

        protected HolderProxy GetHolder(uint holderUid)
        {
            return GalaxyEntry.GetModule<HolderManager>().GetHolder(holderUid, GetSystemDefine());
        }
        protected HolderProxy GetHolder(HolderProxy holder)
        {
            return GalaxyEntry.GetModule<HolderManager>().GetHolder(holder.Uid, GetSystemDefine());
        }

        protected abstract GalaxySystem GetGalaxySystem();
        public SystemType GetSystemType() => GetGalaxySystem().GetSystemType();
        public SystemTypeDefine GetSystemDefine() => GetGalaxySystem().GetSystemDefine();
    }

    public sealed class EmptySystemProxy : GalaxySystemProxy
    {
        public EmptySystemProxy(GalaxySystem system)
        {
        }
        protected override GalaxySystem GetGalaxySystem()
        {
            throw new NotImplementedException();
        }
    }
}
