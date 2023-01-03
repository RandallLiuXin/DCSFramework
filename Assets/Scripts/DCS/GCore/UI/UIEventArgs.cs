namespace Galaxy.Event
{
    //TODO Randall 这里的loading ui事件需要替换成实际的实现
    public sealed class UIUpdateLoadingProgress : UIEventArgs
    {
        public static readonly int EventId = typeof(UIUpdateLoadingProgress).GetHashCode();

        public override int Id => EventId;

        public UIUpdateLoadingProgress(int intervalNum)
        {
            m_IntervalNum = intervalNum;
        }

        public override void Clear()
        {
            m_IntervalNum = 0;
        }

        private int m_IntervalNum;
    }
}
