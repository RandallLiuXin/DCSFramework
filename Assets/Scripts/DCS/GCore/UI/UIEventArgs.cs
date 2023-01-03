namespace Galaxy.Event
{
    //TODO Randall �����loading ui�¼���Ҫ�滻��ʵ�ʵ�ʵ��
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
