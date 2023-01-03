using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace Anvil.Property
{
    public partial class PropertySystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<DCS.DCSInitializeTag, PropertyComponent, PropertyChangeInfoComponent>().ForEach((ref DCS.DCSInitializeTag initializeTag, ref PropertyComponent propertyData, ref PropertyChangeInfoComponent changeInfoData) =>
            {
                if (initializeTag.m_HasInitProperty)
                    return;

                propertyData.Hp = propertyData.MaxHp;
                initializeTag.m_HasInitProperty = true;
            }).ScheduleParallel();
        }
    }
}
