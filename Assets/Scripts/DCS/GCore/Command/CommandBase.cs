using Galaxy.Data;
using Galaxy.Entities;
using Galaxy.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Command
{
    public abstract class CommandBase
    {
        /// <summary>
        /// ��ȡCommand Type(ͨ��moldͳһ����)
        /// </summary>
        public virtual CommandType GetCommandType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ���command��������ݶ���(ͨ��moldͳһ����)
        /// </summary>
        public virtual HolderTypeDefine GetHolderTypeDefine()
        {
            throw new NotImplementedException();
        }

        //�����������ڿͻ��˵����б���
        public abstract void Predict(HolderProxy holder, HolderCommandArgs args);
        public abstract void RollBack(HolderProxy holder, HolderCommandArgs args);

        //ʵ���߼������
        public abstract void Do(HolderProxy holder, HolderCommandArgs args);
        public abstract void Revert(HolderProxy holder, HolderCommandArgs args);
    }

    //������/�ͻ���ģʽʹ��
    public abstract class CommandClient : CommandBase
    {

    }

    public abstract class CommandServer : CommandBase
    {
        public override void Predict(HolderProxy holder, HolderCommandArgs args)
        {
            throw new NotSupportedException();
        }

        public override void RollBack(HolderProxy holder, HolderCommandArgs args)
        {
            throw new NotSupportedException();
        }
    }

    //����ģʽʹ��
    public abstract class CommandCommon : CommandBase
    {
        public override void Predict(HolderProxy holder, HolderCommandArgs args)
        {
            throw new NotSupportedException();
        }

        public override void RollBack(HolderProxy holder, HolderCommandArgs args)
        {
            throw new NotSupportedException();
        }
    }
}
