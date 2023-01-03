using UnityEngine;
using Unity.Entities;

namespace Anvil.Combat
{
    public sealed class FactionComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var factionEntity = dstManager.CreateEntity(new ComponentType[1] { ComponentType.ReadWrite<FactionTag>() });
            dstManager.AddBuffer<FactionData>(factionEntity);

#if UNITY_EDITOR
            dstManager.SetName(factionEntity, "faction pool");
#endif
        }
    }
}
