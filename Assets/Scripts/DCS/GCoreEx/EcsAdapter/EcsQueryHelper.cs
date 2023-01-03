using Unity.Entities;
using Unity.Transforms;

namespace Galaxy.Dots
{
    //TODO Randall 需要改成mold生成的
    public class EcsQueryHelper
    {
        public static EcsQueryArgs GetEcsQuery(EcsQueryType queryType)
        {
            switch (queryType)
            {
                case EcsQueryType.EQ_PlayerTranslation:
                    return new EcsQueryPlayerDefault(queryType);
                case EcsQueryType.EQ_EntityState:
                    return new EcsQueryEntities(queryType);
                default:
                    return null;
            }
        }

        public static EntityQueryDesc GetEntityQueryDesc(EcsQueryType queryType)
        {
            switch (queryType)
            {
                case EcsQueryType.EQ_PlayerTranslation:
                    return new EntityQueryDesc
                    {
                        All = new[]
                        {
                            ComponentType.ReadOnly<Anvil.Common.PlayerTag>(),
                            ComponentType.ReadOnly<Anvil.DCS.DCSEntityTag>(),
                            ComponentType.ReadOnly<Translation>(),
                            ComponentType.ReadOnly<Rotation>()
                        }
                    };
                case EcsQueryType.EQ_EntityState:
                    return new EntityQueryDesc
                    {
                        All = new[] 
                        { 
                            ComponentType.ReadOnly<Anvil.DCS.DCSEntityTag>(),
                            ComponentType.ReadOnly<Anvil.State.StateComponent>()
                        }
                    };
                default:
                    return null;
            }
        }

        public static ComponentType[] GetEntityFilters(EcsQueryType queryType)
        {
            switch (queryType)
            {
                case EcsQueryType.EQ_PlayerTranslation:
                    return new ComponentType[]
                    {
                        ComponentType.ReadOnly<Translation>(),
                        ComponentType.ReadOnly<Rotation>()
                    };
                case EcsQueryType.EQ_EntityState:
                    return new ComponentType[]
                    {
                        ComponentType.ReadOnly<Anvil.DCS.DCSEntityTag>(),
                        ComponentType.ReadOnly<Anvil.State.StateComponent>()
                    };
                default:
                    return null;
            }
        }
    }
}
