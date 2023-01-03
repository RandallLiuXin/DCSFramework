using Unity.Entities;

namespace Anvil.Combat
{
    public struct FactionTag : IComponentData { }

    public struct FactionData : IBufferElementData
    {
        public Entity FactionEntity;
        public uint FactionId;
    }

    public class FactionHelper
    {
        public static bool IsEnemy(uint lhsFaction, uint rhsFaction)
        {
            return lhsFaction != rhsFaction;
        }
    }
}
