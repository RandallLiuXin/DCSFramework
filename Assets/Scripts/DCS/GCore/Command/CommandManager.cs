using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Entities;
using Galaxy.Data;
using Galaxy.Event;

namespace Galaxy.Command
{
    /// <summary>
    /// �����������
    /// 1. ͨ��attribute�Զ�����CommandType��ȡ��Ӧ��class����
    /// 2. ͨ��attribute�Զ�����CommandType��ȡ��Ӧ��systemDefineType
    /// </summary>
    internal sealed class CommandManager : GalaxyModule
    {
        private Dictionary<CommandType, CommandBase> m_AllCommandInstances;
        /// <summary>
        /// ��ʼ���������������ʵ����
        /// </summary>
        public CommandManager()
        {
            m_AllCommandInstances = CommandHelper.GetAllCommands();
        }

        /// <summary>
        /// �����������ѯ��
        /// </summary>
        /// <param name="elapseSeconds">�߼�����ʱ�䣬����Ϊ��λ��</param>
        internal override void Update(float elapseSeconds)
        {
        }

        /// <summary>
        /// �رղ����������������
        /// </summary>
        internal override void Shutdown()
        {
        }

        #region ForLocalClient
        //����command���û��ߵ�����Ϸʹ��
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
