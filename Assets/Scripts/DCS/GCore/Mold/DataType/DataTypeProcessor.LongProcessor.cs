using System.IO;

namespace Galaxy.Mold
{
    public sealed partial class DataTypeProcessor
    {
        private sealed class LongProcessor : GenericDataProcessor<long>
        {
            public override bool IsSystem
            {
                get
                {
                    return true;
                }
            }

            public override string LanguageKeyword
            {
                get
                {
                    return "long";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "long",
                    "int64",
                    "system.int64"
                };
            }

            public override long Parse(string value)
            {
                return long.Parse(value);
            }

            public override void WriteToStream(BinaryWriter stream, string value)
            {
                stream.Write(Parse(value));
            }
        }
    }
}
