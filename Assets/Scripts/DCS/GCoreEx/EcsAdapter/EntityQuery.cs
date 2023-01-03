using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Galaxy.Dots
{
    public partial struct QueryEntityStateJob : IJobEntity, IEcsQueryJob
    {
        public void InitJob(EcsQueryArgs args)
        {
        }

        public void Execute(Entity entity, in Anvil.DCS.DCSEntityTag entityTag, in Anvil.State.StateComponent stateData)
        {
            var dcsEntity = GalaxyEntry.GetModule<Entities.EntityManager>().GetEntity(entityTag.EntityUid);
            if (dcsEntity == null)
                return;

            if (stateData.DeadFlag.IsTouch() && stateData.DeadFlag.Value)
            {
                dcsEntity.FireFsmEvent(Entities.DeadStateEvent.EventId, new Entities.DeadStateEvent { });
            }
        }
    }
}
