using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Anvil.Combat;
using Anvil.State;

namespace Anvil.Common
{
    public class CombatConversion : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            {
                HurtBoxComponent component = default(HurtBoxComponent);
                dstManager.AddComponentData(entity, component);
                dstManager.AddBuffer<HurtBoxInfo>(entity);
                dstManager.AddBuffer<HurtBoxResult>(entity);
            }

            {
                HitBoxComponent component = default(HitBoxComponent);
                dstManager.AddComponentData(entity, component);
                dstManager.AddBuffer<HitBoxInfo>(entity);
            }

            {
                StateComponent component = default(StateComponent);
                dstManager.AddComponentData(entity, component);
            }
        }
    }
}
