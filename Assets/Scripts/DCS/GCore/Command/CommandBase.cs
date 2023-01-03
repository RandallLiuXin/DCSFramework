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
        /// 获取Command Type(通过mold统一生成)
        /// </summary>
        public virtual CommandType GetCommandType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 相关command所需的数据定义(通过mold统一生成)
        /// </summary>
        public virtual HolderTypeDefine GetHolderTypeDefine()
        {
            throw new NotImplementedException();
        }

        //后续可以用于客户端的先行表现
        public abstract void Predict(HolderProxy holder, HolderCommandArgs args);
        public abstract void RollBack(HolderProxy holder, HolderCommandArgs args);

        //实际逻辑层调用
        public abstract void Do(HolderProxy holder, HolderCommandArgs args);
        public abstract void Revert(HolderProxy holder, HolderCommandArgs args);
    }

    //服务器/客户端模式使用
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

    //单机模式使用
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
