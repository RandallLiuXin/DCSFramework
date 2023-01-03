using Galaxy.Command;
using Galaxy.Common;
using Galaxy.Data;
using Galaxy.Entities;
using Galaxy.Event;
using Galaxy.Fsm;
using Galaxy.ObjectPool;
using Galaxy.Procedure;
using Galaxy.Setting;
using Galaxy.Task;
using Galaxy.Visual;
using Galaxy.Dots;
using System;
using System.Collections.Generic;

namespace Galaxy
{
    public class CoreConst
    {
        public static uint INVAILD_UID = 0;
        public static uint INVAILD_VID = 0;
        public static uint INVAILD_PID = 0;

        public static Dictionary<Type, int> ModulePriority = new Dictionary<Type, int>
        {
            { typeof(ProcedureManager), -10 },
            { typeof(UidGenerator), 0 },
            { typeof(CommandManager), 0 },
            { typeof(SettingManager), 0 },
            { typeof(TaskManager), 0 },
            { typeof(ObjectPoolManager), 90 },
            { typeof(EventManager), 100 },
            { typeof(FsmManager), 120 },

            { typeof(VisualProxyManager), 800 },
            { typeof(VisualProxyRegister), 800 },

            { typeof(GalaxySystemManager), 0 },
            { typeof(EntityManager), 100 },
            { typeof(HolderManager), 500 },

            //extend
            { typeof(EcsAdapterModule), 1000 },
        };
    }
}
