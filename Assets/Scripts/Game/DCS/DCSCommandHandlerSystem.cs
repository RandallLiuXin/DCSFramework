using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Galaxy;
using Anvil.Property;

namespace Anvil.DCS
{
    [UpdateInGroup(typeof(DCSSystemGroup)), UpdateAfter(typeof(DCSUpdateSystem))]
    public partial class DCSCommandHandlerSystem : SystemBase
    {
        EndInitializationEntityCommandBufferSystem m_EndInitECBSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_EndInitECBSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!GalaxyEntry.HasInitialized())
            {
                return;
            }

            var commandBuffer = m_EndInitECBSystem.CreateCommandBuffer().AsParallelWriter();

            //1. dcsʵ�崴��
            Entities.WithAll<DCSInitializeTag>().WithoutBurst().ForEach((Entity entity, int nativeThreadIndex, ref DCSInitializeTag initializeTag) =>
            {
                if (initializeTag.m_HasDCSEntity)
                {
                    return;
                }

                var dcsEntity = GalaxyEntry.GetModule<Galaxy.Entities.EntityManager>().CreateEntity(initializeTag.m_EntityType, initializeTag.m_EntityConfigId);
                var holder = GalaxyEntry.GetModule<Galaxy.Data.HolderManager>().GetHolder(dcsEntity.UID);
                var Vid = holder.GetComponent<Galaxy.Visual.VisualComponent>().VisualPid;

                commandBuffer.AddComponent(nativeThreadIndex, entity, new DCSEntityTag { EntityUid = dcsEntity.UID, EntityVid = Vid });

                //gameplay init
                {
                    var factionEntity = GetSingletonEntity<Combat.FactionTag>();
                    var factionBuffer = GetBufferFromEntity<Combat.FactionData>()[factionEntity];
                    //TODO ���ݻ�ȡ
                    factionBuffer.Add(new Combat.FactionData { FactionEntity = entity, FactionId = initializeTag.m_EntityConfigId });
                }

                initializeTag.m_HasDCSEntity = true;
            }).Run();

            //2. ����ϵͳ�Ƿ��ʼ�����
            Entities.WithAll<DCSInitializeTag>().WithoutBurst().ForEach((Entity entity, int nativeThreadIndex, ref DCSInitializeTag initializeTag) =>
            {
                if (!initializeTag.HasInitAllComponent())
                {
                    return;
                }

                commandBuffer.RemoveComponent<DCSInitializeTag>(nativeThreadIndex, entity);
            }).Run();

            //3. ��ǰdcsָ���
            Entities.WithoutBurst().ForEach((Entity entity, int nativeThreadIndex, ref DCSEntityTag entityTag) =>
            {
                if (entityTag.EntityUid == CoreConst.INVAILD_UID)
                {
                    return;
                }

                var ecsCommands = GalaxyEntry.GetModule<Galaxy.Dots.EcsAdapterModule>().GetEcsCommands(entityTag.EntityUid);
                if (ecsCommands == null)
                {
                    return;
                }

                foreach (var item in ecsCommands)
                {
                    item.RunOperation(commandBuffer, nativeThreadIndex, entity);
                }
                GalaxyEntry.GetModule<Galaxy.Dots.EcsAdapterModule>().RemoveEcsCommands(entityTag.EntityUid);
            }).Run();

            //4. dcsʵ������
            Entities.ForEach((Entity entity, int nativeThreadIndex, in PropertyComponent propertyData) =>
            {
                if (propertyData.Hp <= 0.0f)
                {
                    //TODO ������������
                    //commandBuffer.DestroyEntity(nativeThreadIndex, entity);
                }
            }).Run();

            m_EndInitECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
