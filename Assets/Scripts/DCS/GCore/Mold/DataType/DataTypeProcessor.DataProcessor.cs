using System.IO;

namespace Galaxy.Mold
{
    public sealed partial class DataTypeProcessor
    {
        public abstract class DataProcessor
        {
            public abstract System.Type Type
            {
                get;
            }

            public abstract bool IsId
            {
                get;
            }

            public abstract bool IsComment
            {
                get;
            }

            public abstract bool IsSystem
            {
                get;
            }

            public abstract string LanguageKeyword
            {
                get;
            }

            public abstract string[] GetTypeStrings();

            public abstract void WriteToStream(BinaryWriter stream, string value);
        }
    }
}
