namespace Galaxy.Mold
{
    public sealed partial class DataTypeProcessor
    {
        public abstract class GenericDataProcessor<T> : DataProcessor
        {
            public override System.Type Type
            {
                get
                {
                    return typeof(T);
                }
            }

            public override bool IsId
            {
                get
                {
                    return false;
                }
            }

            public override bool IsComment
            {
                get
                {
                    return false;
                }
            }

            public abstract T Parse(string value);
        }
    }
}
