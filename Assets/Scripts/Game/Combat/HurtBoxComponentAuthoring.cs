using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Anvil.Combat
{
    public class HurtBoxComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            HurtBoxComponent component = default(HurtBoxComponent);
            dstManager.AddComponentData(entity, component);
            dstManager.AddBuffer<HurtBoxInfo>(entity);
            dstManager.AddBuffer<HurtBoxResult>(entity);
        }
    }
}

