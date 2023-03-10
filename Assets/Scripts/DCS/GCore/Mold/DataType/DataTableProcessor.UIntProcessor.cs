using System.IO;

namespace Galaxy.Mold
{
    public sealed partial class DataTypeProcessor
    {
        private sealed class UIntProcessor : GenericDataProcessor<uint>
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
                    return "uint";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "uint",
                    "uint32",
                    "system.uint32"
                };
            }

            public override uint Parse(string value)
            {
                return uint.Parse(value);
            }

            public override void WriteToStream(BinaryWriter stream, string value)
            {
                stream.Write(Parse(value));
            }
        }
    }
}
