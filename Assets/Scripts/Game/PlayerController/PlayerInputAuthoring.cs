using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Anvil.Player
{
    [DisallowMultipleComponent]
    public class PlayerInputAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            PlayerInput component = default(PlayerInput);
            component.m_NeedRefreshInput = false;
            component.m_InputVector = float3.zero;
            dstManager.AddComponentData(entity, component);
        }
    }
}
