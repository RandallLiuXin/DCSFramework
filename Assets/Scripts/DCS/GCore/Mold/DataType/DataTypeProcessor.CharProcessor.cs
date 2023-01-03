using System.IO;

namespace Galaxy.Mold
{
    public sealed partial class DataTypeProcessor
    {
        private sealed class CharProcessor : GenericDataProcessor<char>
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
                    return "char";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "char",
                    "system.char"
                };
            }

            public override char Parse(string value)
            {
                return char.Parse(value);
            }

            public override void WriteToStream(BinaryWriter stream, string value)
            {
                stream.Write(Parse(value));
            }
        }
    }
}
