using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace Galaxy.Dots
{
    public partial struct QueryPlayerTranslationJob : IJobEntity, IEcsQueryJob
    {
        public void InitJob(EcsQueryArgs args)
        {
        }

        public void Execute(in Anvil.Common.PlayerTag playerTag, in Anvil.DCS.DCSEntityTag entityTag, in Translation translation, in Rotation rotation)
        {
            {
                Visual.Command.EngineCommandUpdateCameraTargetPos cmd = new Visual.Command.EngineCommandUpdateCameraTargetPos
                {
                    CameraTargetPos = translation.Value
                };
                GalaxyEntry.GetModule<Visual.VisualProxyManager>().EngineCommand(cmd);
            }
        }
    }
}
