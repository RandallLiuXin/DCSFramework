using System.IO;
using UnityEngine;

namespace Galaxy.Mold
{
    public sealed partial class DataTypeProcessor
    {
        private sealed class Vector2Processor : GenericDataProcessor<Vector2>
        {
            public override bool IsSystem
            {
                get
                {
                    return false;
                }
            }

            public override string LanguageKeyword
            {
                get
                {
                    return "Vector2";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "vector2",
                    "unityengine.vector2"
                };
            }

            public override Vector2 Parse(string value)
            {
                string[] splitValue = value.Split(',');
                return new Vector2(float.Parse(splitValue[0]), float.Parse(splitValue[1]));
            }

            public override void WriteToStream(BinaryWriter stream, string value)
            {
                Vector2 vector2 = Parse(value);
                stream.Write(vector2.x);
                stream.Write(vector2.y);
            }
        }
    }
}
