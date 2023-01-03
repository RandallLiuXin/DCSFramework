using System;
using System.Collections.Generic;

namespace Galaxy
{
    /// <summary>
    /// 1. 当前结构目标保留ECS的运行高效和OOP的开发高效。因此：
    ///     与ECSProxy间为保留引用，ECSEntity会正常的按照ECS的运行逻辑执行，
    ///     而在OOP中可以只读的方式获取对应的ECSEntity的数据并赋值给Entity中的OOPComponent。
    ///     这样做法下冗余为OOP获取值并赋值给OOP/两份数据内存
    ///     
    /// 2. 如果需要联机、DS服务器，则需要在这里添加puppet/master标识。并处理holder manager中的数据
    /// 
    /// 3. 为什么要完全舍弃unity的entity-component结构：
    ///     方便后续更换引擎并兼容ECS架构。ECS架构并不能使用unity自带的entity-component结构。所以必须完全拆分开来解耦合
    ///     
    /// 4. 数据、逻辑分离，逻辑定义在相关的system中，数据定义在holder中。
    ///     这样在处理网络同步时，属性同步可以直接通过holder完成。事件同步则通过system完成
    ///     
    /// 5. 表现、逻辑分离
    ///     所有表现层的工作都是强关联引擎的，因此定义了VisualProxy来管理所有的Visual层。
    ///     如果有什么表现相关的功能，需要按照Unity原本方法完成，即添加monobehavior完成。
    ///     逻辑层保有表现层的唯一ID，并通过Cmd的方式控制表现层表现。这样在之后替换引擎时只需要更换所有visual层代码即可
    /// </summary>
    public static class GalaxySummary
    {
    }

    /// <summary>
    /// 游戏框架入口。
    /// </summary>
    public static class GalaxyEntry
    {
        private static readonly LinkedList<GalaxyModule> s_GalaxyModules = new LinkedList<GalaxyModule>();

        public static GameManagerInstance ms_GameManagerInstance;

        public static bool ms_GameIsRelease = true;

        private static bool ms_Initialized = false;
        public static bool HasInitialized() => ms_Initialized;

        /// <summary>
        /// 初始化目标引擎的相关模块
        /// </summary>
        public static void InitEngine(GameManagerInstance instance)
        {
#if UNITY_EDITOR
            ms_GameIsRelease = false;
#endif
            ms_GameManagerInstance = instance;

            instance.gameObject.AddComponent<Timer.TimerManager>();
            instance.gameObject.AddComponent<Timer.RoutineRunner>();
        }

        /// <summary>
        /// 启动游戏框架模块
        /// </summary>
        public static void Init()
        {
            GetModule<Common.UidGenerator>();
            GetModule<Data.HolderManager>();
            GetModule<Entities.EntityManager>();
            GetModule<Entities.GalaxySystemManager>();
            GetModule<Event.EventManager>();
            GetModule<Fsm.FsmManager>();
            GetModule<Procedure.ProcedureManager>();
            GetModule<Task.TaskManager>();
            GetModule<Setting.SettingManager>();
            GetModule<ObjectPool.ObjectPoolManager>();
            GetModule<Dots.EcsAdapterModule>();

            ms_Initialized = true;
        }

        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        public static void UpdateLogic(float elapseSeconds)
        {
            foreach (GalaxyModule module in s_GalaxyModules)
            {
                if (!module.NeedUpdateLogic)
                {
                    continue;
                }
                module.Update(elapseSeconds);
            }
        }

        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        public static void UpdateMono(float elapseSeconds)
        {
            foreach (GalaxyModule module in s_GalaxyModules)
            {
                if (!module.NeedUpdateMono)
                {
                    continue;
                }
                module.Update(elapseSeconds);
            }
        }

        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        public static void FixedUpdateMono(float elapseSeconds)
        {
            foreach (GalaxyModule module in s_GalaxyModules)
            {
                if (!module.NeedFixedUpdate)
                {
                    continue;
                }
                module.FixedUpdate(elapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理所有游戏框架模块。
        /// </summary>
        public static void Shutdown()
        {
            for (LinkedListNode<GalaxyModule> current = s_GalaxyModules.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }

            s_GalaxyModules.Clear();
            ReferencePool.ClearAll();
            GalaxyLog.SetLogHelper(null);
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架模块类型。</typeparam>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        public static T GetModule<T>() where T : class
        {
            Type interfaceType = typeof(T);

            if (!interfaceType.FullName.StartsWith("Galaxy."))
            {
                throw new GalaxyException(Utility.Text.Format("You must get a Game Framework module, but '{0}' is not.", interfaceType.FullName));
            }

            string moduleName = Utility.Text.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name);
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
            {
                throw new GalaxyException(Utility.Text.Format("Can not find Game Framework module type '{0}'.", moduleName));
            }

            return GetModule(moduleType) as T;
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要获取的游戏框架模块类型。</param>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        private static GalaxyModule GetModule(Type moduleType)
        {
            foreach (GalaxyModule module in s_GalaxyModules)
            {
                if (module.GetType() == moduleType)
                {
                    return module;
                }
            }

            return CreateModule(moduleType);
        }

        /// <summary>
        /// 创建游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要创建的游戏框架模块类型。</param>
        /// <returns>要创建的游戏框架模块。</returns>
        private static GalaxyModule CreateModule(Type moduleType)
        {
            GalaxyModule module = (GalaxyModule)Activator.CreateInstance(moduleType);
            if (module == null)
            {
                throw new GalaxyException(Utility.Text.Format("Can not create module '{0}'.", moduleType.FullName));
            }

            LinkedListNode<GalaxyModule> current = s_GalaxyModules.First;
            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                s_GalaxyModules.AddBefore(current, module);
            }
            else
            {
                s_GalaxyModules.AddLast(module);
            }

            return module;
        }

#if UNITY_EDITOR
        public static Dictionary<uint, Data.Holder> GetAllData()
        {
            return new Dictionary<uint, Data.Holder>(GetModule<Data.HolderManager>().GetAllData());
        }

        public static Entities.EntityType GetEntityType(uint holderUid)
        {
            return GetModule<Entities.EntityManager>().GetEntity(holderUid).GetEntityType();
        }
#endif
    }
}
