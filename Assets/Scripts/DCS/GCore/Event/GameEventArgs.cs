namespace Galaxy.Event
{
    /// <summary>
    /// 游戏逻辑事件基类。
    /// </summary>
    public abstract class GameEventArgs : BaseEventArgs
    {
    }

    /// <summary>
    /// UI事件基类。
    /// </summary>
    public abstract class UIEventArgs : GameEventArgs
    {
    }

    /// <summary>
    /// VisualCmd基类。
    /// </summary>
    public abstract class VisualCommandArgs : GameEventArgs
    {
        /// <summary>
        /// 获取Visual Command Type。
        /// </summary>
        public abstract Visual.VisualCommandType CommandType
        {
            get;
        }
    }

    /// <summary>
    /// VisualQuery基类。
    /// </summary>
    public abstract class VisualQueryArgs : GameEventArgs
    {
        /// <summary>
        /// 获取Visual Command Type。
        /// </summary>
        public abstract Visual.VisualQueryType QueryType
        {
            get;
        }
    }

    /// <summary>
    /// EngineCmd基类。
    /// </summary>
    public abstract class EngineCommandArgs : GameEventArgs
    {
        /// <summary>
        /// 获取Engine Command Type。
        /// </summary>
        public abstract Visual.EngineCommandType CommandType
        {
            get;
        }
    }

    /// <summary>
    /// EngineQuery基类。
    /// </summary>
    public abstract class EngineQueryArgs : GameEventArgs
    {
        /// <summary>
        /// 获取Engine Command Type。
        /// </summary>
        public abstract Visual.EngineQueryType QueryType
        {
            get;
        }
    }

    /// <summary>
    /// Holder Command基类。
    /// </summary>
    public abstract class HolderCommandArgs : GameEventArgs
    {
        public abstract Command.CommandType CommandType
        {
            get;
        }
    }
}
