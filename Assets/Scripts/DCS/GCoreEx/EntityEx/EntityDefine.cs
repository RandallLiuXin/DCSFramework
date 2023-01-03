using System;
using System.Collections.Generic;
using Galaxy.Mold;

namespace Galaxy.Entities
{
    public enum EntityType
    {
        LocalPlayer,
        AIEntity,
        Count
    }

    public class EntityDefine
    {
        public static Type[] EntityTypes = new Type[(int)EntityType.Count]
        {
            typeof(LocalPlayer),
            typeof(AIEntity),
        };
    }

    internal sealed partial class EntityManager
    {
        private void InternalCollectEntityTypeDefine()
        {
            //////////////////////////////////////
            //extend
            {
                //本地单位，用于本地玩家数据保存，如本地玩家输入数据等
                EntityTypeDefine entityDefine = new EntityTypeDefine();
                entityDefine.m_EntityType = EntityType.LocalPlayer;
                entityDefine.m_SystemTypes = new SystemType[] { SystemType.Visual, SystemType.Animation };
                entityDefine.m_CompTypes = new CompType[] { CompType.Visual, CompType.Animation };
                ms_EntityType2AllComponentMap.Add(EntityType.LocalPlayer, entityDefine);
            }
            {
                EntityTypeDefine entityDefine = new EntityTypeDefine();
                entityDefine.m_EntityType = EntityType.AIEntity;
                entityDefine.m_SystemTypes = new SystemType[] { SystemType.Visual, SystemType.Animation };
                entityDefine.m_CompTypes = new CompType[] { CompType.Visual, CompType.Animation };
                ms_EntityType2AllComponentMap.Add(EntityType.AIEntity, entityDefine);
            }
        }
    }

    [GalaxyEntity(EntityType.LocalPlayer)]
    public class LocalPlayer : Entity
    {
        public LocalPlayer(uint id, SystemType[] systems) : base(id, systems)
        {

        }

        public override EntityType GetEntityType()
        {
            return EntityType.LocalPlayer;
        }

        protected override EntityFsm CreateEntityFsm()
        {
            return new LocalPlayerFsm(this);
        }
    }

    public class LocalPlayerFsm : EntityFsm
    {
        public LocalPlayerFsm(Entity owner) : base(owner)
        {
        }

        public override Type GetDefaultState()
        {
            return typeof(LocalPlayerStateIdle);
        }

        public override EntityBaseState[] GetEntityAllStates()
        {
            return new EntityBaseState[] {
                new LocalPlayerStateIdle(),
                new LocalPlayerStateMoving(),
                new LocalPlayerStateSkill(),
                new LocalPlayerStateDead(),
            };
        }
    }

    //TODO Randall temp ai entity
    [GalaxyEntity(EntityType.AIEntity)]
    public class AIEntity : Entity
    {
        public AIEntity(uint id, SystemType[] systems) : base(id, systems)
        {

        }

        public override EntityType GetEntityType()
        {
            return EntityType.AIEntity;
        }

        protected override EntityFsm CreateEntityFsm()
        {
            return new EmptyEntityFsm(this);
        }
    }

    public class EmptyEntityFsm : EntityFsm
    {
        public EmptyEntityFsm(Entity owner) : base(owner)
        {
        }

        public override Type GetDefaultState()
        {
            return typeof(EntityStateIdle);
        }

        public override EntityBaseState[] GetEntityAllStates()
        {
            return new EntityBaseState[] {
                new EntityStateIdle(),
                new EntityStateMoving(),
                new EntityStateDead(),
            };
        }
    }
}
