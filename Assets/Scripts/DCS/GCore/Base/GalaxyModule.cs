namespace Galaxy
{
    /// <summary>
    /// 游戏框架模块抽象类。
    /// </summary>
    internal abstract class GalaxyModule
    {
        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal int Priority
        {
            get
            {
                return CoreConst.ModulePriority[GetType()];
            }
        }

        internal virtual bool NeedUpdateLogic => true;
        internal virtual bool NeedUpdateMono => false;
        internal virtual bool NeedFixedUpdate => false;

        /// <summary>
        /// 游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        internal abstract void Update(float elapseSeconds);

        /// <summary>
        /// 游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        internal virtual void FixedUpdate(float elapseSeconds) { }

        /// <summary>
        /// 关闭并清理游戏框架模块。
        /// </summary>
        internal abstract void Shutdown();
    }
}
