using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Galaxy.Dots;

namespace Anvil.DCS
{
    [UpdateInGroup(typeof(DCSSystemGroup), OrderFirst = true)]
    public partial class DCSDataCollectorSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (!Galaxy.GalaxyEntry.HasInitialized())
            {
                return;
            }

            var dependency = Dependency;

            var adapterModule = Galaxy.GalaxyEntry.GetModule<EcsAdapterModule>();
            Dictionary<EcsQueryType, EcsQueryArgs> queries = new Dictionary<EcsQueryType, EcsQueryArgs>(adapterModule.GetCurrentEcsQueries());
            foreach (var item in queries)
            {
                var queryType = item.Key;
                var query = item.Value;

                var entityQuery = GetEntityQuery(EcsQueryHelper.GetEntityQueryDesc(queryType));
                entityQuery.SetChangedVersionFilter(EcsQueryHelper.GetEntityFilters(queryType));

                var entityCount = entityQuery.CalculateEntityCount();
                if (entityCount <= 0)
                    continue;

                switch (queryType)
                {
                    case EcsQueryType.EQ_PlayerTranslation:
                        {
                            var job = new QueryPlayerTranslationJob();
                            job.InitJob(query);
                            dependency = job.Schedule(entityQuery, dependency);
                        }
                        break;
                    case EcsQueryType.EQ_EntityState:
                        {
                            var job = new QueryEntityStateJob();
                            job.InitJob(query);
                            dependency = job.Schedule(entityQuery, dependency);
                        }
                        break;
                    default:
                        break;
                }
            }

            Dependency = dependency;
        }
    }
}
