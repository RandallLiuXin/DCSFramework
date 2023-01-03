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
    /// 框架系统基类
    /// </summary>
    public abstract class GalaxySystem
    {
        public GalaxySystem()
        {

        }

        /// <summary>
        /// 返回自己定义在SystemDefine中的枚举
        /// </summary>
        /// <returns></returns>
        public SystemType GetSystemType() => GetSystemDefine().m_SystemType;

        /// <summary>
        /// 返回自身的数据定义
        /// </summary>
        /// <returns></returns>
        public abstract SystemTypeDefine GetSystemDefine();

        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="uid">对象id</param>
        internal abstract void Init(HolderProxy holder);

        /// <summary>
        /// 初始化对象完成
        /// </summary>
        /// <param name="uid">对象id</param>
        internal abstract void Initialize(HolderProxy holder);

        /// <summary>
        /// 轮询当前对象
        /// </summary>
        /// <param name="uid">对象id</param>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        internal abstract void Update(HolderProxy holder, float elapseSeconds);

        /// <summary>
        /// 轮询当前对象
        /// </summary>
        /// <param name="uid">对象id</param>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        internal abstract void PostUpdate(HolderProxy holder, float elapseSeconds);

        /// <summary>
        /// 轮询当前对象
        /// </summary>
        /// <param name="uid">对象id</param>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        internal abstract void FixedUpdate(HolderProxy holder, float elapseSeconds);

        /// <summary>
        /// 移除对象之前
        /// </summary>
        /// <param name="uid">对象id</param>
        internal abstract void PreDestroy(HolderProxy holder);

        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="uid">对象id</param>
        internal abstract void Destroy(HolderProxy holder);
    }

    /// <summary>
    /// Oop系统
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
