using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Entities;
using Galaxy.Data;
using Galaxy.Event;

namespace Galaxy.Command
{
    /// <summary>
    /// 命令管理器。
    /// 1. 通过attribute自动生成CommandType获取对应的class对象
    /// 2. 通过attribute自动生成CommandType获取对应的systemDefineType
    /// </summary>
    internal sealed class CommandManager : GalaxyModule
    {
        private Dictionary<CommandType, CommandBase> m_AllCommandInstances;
        /// <summary>
        /// 初始化命令管理器的新实例。
        /// </summary>
        public CommandManager()
        {
            m_AllCommandInstances = CommandHelper.GetAllCommands();
        }

        /// <summary>
        /// 命令管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds)
        {
        }

        /// <summary>
        /// 关闭并清理命令管理器。
        /// </summary>
        internal override void Shutdown()
        {
        }

        #region ForLocalClient
        //本地command调用或者单机游戏使用
        public void PushCommand(uint holderUid, HolderCommandArgs args)
        {
            CommandType commandType = args.CommandType;
            Debug.Assert(m_AllCommandInstances.ContainsKey(commandType));
            CommandBase command = m_AllCommandInstances[commandType];
            Debug.Assert(command != null);

            HolderTypeDefine holderDefine = command.GetHolderTypeDefine();
            HolderProxy holder = GalaxyEntry.GetModule<HolderManager>().GetHolder(holderUid, holderDefine);
            Debug.Assert(holder != null);

            command.Do(holder, args);
        }

        public void RevertCommand(uint holderUid, HolderCommandArgs args)
        {
            CommandType commandType = args.CommandType;
            Debug.Assert(m_AllCommandInstances.ContainsKey(commandType));
            CommandBase command = m_AllCommandInstances[commandType];
            Debug.Assert(command != null);

            HolderTypeDefine holderDefine = command.GetHolderTypeDefine();
            HolderProxy holder = GalaxyEntry.GetModule<HolderManager>().GetHolder(holderUid, holderDefine);
            Debug.Assert(holder != null);

            command.Revert(holder, args);
        }
        #endregion

        #region ForServer
        //public void DispatchCommandToAOI(uint holderUid, CommandType commandType, HolderCommandArgs args)
        //{
        //}
        //public void DispatchCommandToMyClient(uint holderUid, CommandType commandType, HolderCommandArgs args)
        //{
        //}
        //public void DispatchCommandToTargetClinet(uint holderUid, CommandType commandType, HolderCommandArgs args)
        //{
        //}
        #endregion

        #region ForClient
        //public void DispatchCommandToMyServer(uint holderUid, CommandType commandType, HolderCommandArgs args)
        //{
        //}
        //public void DispatchCommandToTargetServer(uint holderUid, CommandType commandType, HolderCommandArgs args)
        //{
        //}
        #endregion
    }
}
