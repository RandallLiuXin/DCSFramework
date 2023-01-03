using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Anvil.DCS
{
    [UpdateInGroup(typeof(DCSSystemGroup)), UpdateAfter(typeof(DCSDataCollectorSystem))]
    public partial class DCSUpdateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (!Galaxy.GalaxyEntry.HasInitialized())
            {
                return;
            }

            float deltaTime = Time.DeltaTime;
            Galaxy.GalaxyEntry.UpdateLogic(deltaTime);
        }
    }
}
