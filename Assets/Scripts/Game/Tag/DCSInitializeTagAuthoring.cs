using UnityEngine;
using Unity.Entities;

namespace Anvil.DCS
{
    public class DCSInitializeTagAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [RegisterBinding(typeof(DCSInitializeTag), "m_EntityType")]
        public Galaxy.Entities.EntityType m_EntityType;
        [RegisterBinding(typeof(DCSInitializeTag), "m_EntityConfigId")]
        public uint m_EntityConfigId;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            DCSInitializeTag component = default(DCSInitializeTag);
            component.m_EntityType = m_EntityType;
            component.m_EntityConfigId = m_EntityConfigId;
            component.m_HasDCSEntity = false;
            component.m_HasInitHitBox = gameObject.GetComponent<Common.CombatConversion>() == null;
            component.m_HasInitProperty = gameObject.GetComponent<Property.PropertyComponentAuthoring>() == null;
            component.m_HasInitAnimation = gameObject.GetComponent<Animation.AnimationComponentAuthoring>() == null;
            dstManager.AddComponentData(entity, component);
        }
    }
}
