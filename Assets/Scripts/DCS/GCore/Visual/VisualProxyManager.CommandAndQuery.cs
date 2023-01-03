using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Visual
{
    internal sealed partial class VisualProxyManager
    {
        //private readonly Dictionary<int, LinkedList<EventHandler<T>>> m_EventHandlers;

        private void InitCommandAndQueryPart()
        {
            InitCommandToFlushMap();
            InitCommand();
            InitFlush();
            InitQuery();
        }

        private void InitCommandToFlushMap()
        {
            //Visual Base
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(VisualCommandType.Position, new List<VisualFlushType> { VisualFlushType.Position });
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(VisualCommandType.Rotation, new List<VisualFlushType> { VisualFlushType.Rotation });
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(VisualCommandType.Scale, new List<VisualFlushType> { VisualFlushType.Scale });

            //Model Proxy
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(VisualCommandType.SetAnimatorParameter, new List<VisualFlushType> { VisualFlushType.SetAnimatorParameter });
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(VisualCommandType.SetAnimatorEventCallback, new List<VisualFlushType> { VisualFlushType.SetAnimatorEventCallback });
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(VisualCommandType.SetAnimatorRootMotionCallback, new List<VisualFlushType> { VisualFlushType.SetAnimatorRootMotionCallback });
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(VisualCommandType.ToggleAnimatorRootMotion, new List<VisualFlushType> { VisualFlushType.ToggleAnimatorRootMotion });
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(VisualCommandType.ToggleAnimator, new List<VisualFlushType> { VisualFlushType.ToggleAnimator });

            //Engine proxy
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(EngineCommandType.LoadScene, new List<EngineFlushType> { EngineFlushType.ActiveScene });
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommandToFlush(EngineCommandType.UpdateCameraTargetPos, new List<EngineFlushType> { });
        }

        private void InitCommand()
        {
            //Visual Base
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(VisualCommandType.Position, VisualBase.UpdatePosition);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(VisualCommandType.Rotation, VisualBase.UpdateRotation);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(VisualCommandType.Scale, VisualBase.UpdateScale);

            //Model Proxy
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(VisualCommandType.SetAnimatorParameter, ModelProxy.SetAnimatorParameter);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(VisualCommandType.SetAnimatorEventCallback, ModelProxy.SetAnimatorEventCallback);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(VisualCommandType.SetAnimatorRootMotionCallback, ModelProxy.SetAnimatorRootMotionCallback);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(VisualCommandType.ToggleAnimatorRootMotion, ModelProxy.ToggleAnimatorRootMotion);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(VisualCommandType.ToggleAnimator, ModelProxy.ToggleAnimator);

            //Engine proxy
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(EngineCommandType.LoadScene, EngineProxy.LoadScene);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterCommand(EngineCommandType.UpdateCameraTargetPos, EngineProxy.UpdateCameraTargetPos);
        }

        private void InitFlush()
        {
            //Visual Base
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterFlush(VisualFlushType.Position, VisualBase.FlushPosition);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterFlush(VisualFlushType.Rotation, VisualBase.FlushRotation);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterFlush(VisualFlushType.Scale, VisualBase.FlushScale);

            //Model Proxy
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterFlush(VisualFlushType.SetAnimatorParameter, ModelProxy.FlushSetAnimatorParameter);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterFlush(VisualFlushType.SetAnimatorEventCallback, ModelProxy.FlushSetAnimatorEventCallback);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterFlush(VisualFlushType.SetAnimatorRootMotionCallback, ModelProxy.FlushSetAnimatorRootMotionCallback);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterFlush(VisualFlushType.ToggleAnimatorRootMotion, ModelProxy.FlushToggleAnimatorRootMotion);
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterFlush(VisualFlushType.ToggleAnimator, ModelProxy.FlushToggleAnimator);

            //Engine proxy
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterFlush(EngineFlushType.ActiveScene, EngineProxy.ActiveScene);
        }

        private void InitQuery()
        {
            //Visual Base
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterQuery(new Query.VisualQueryPosition());
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterQuery(new Query.VisualQueryRotation());
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterQuery(new Query.VisualQueryGetSkeleton());

            //Engine proxy
            GalaxyEntry.GetModule<VisualProxyRegister>().RegisterQuery(new Query.EngineQueryGetScene());
        }

        private void ProcessCommand(VisualBase visual, Event.VisualCommandArgs visualCommand)
        {
            Debug.Assert(visualCommand != null);
            var handler = GalaxyEntry.GetModule<VisualProxyRegister>().GetCommandAction(visualCommand.CommandType);
            handler(visual, visualCommand);

            var flushTypes = GalaxyEntry.GetModule<VisualProxyRegister>().GetRelatedFlushs(visualCommand.CommandType);
            foreach (var flushType in flushTypes)
                visual.AddFlushHandler(flushType);
        }

        private void ProcessCommand(EngineProxy engine, Event.EngineCommandArgs engineCommand)
        {
            Debug.Assert(engineCommand != null);
            var handler = GalaxyEntry.GetModule<VisualProxyRegister>().GetCommandAction(engineCommand.CommandType);
            handler(engine, engineCommand);

            var flushTypes = GalaxyEntry.GetModule<VisualProxyRegister>().GetRelatedFlushs(engineCommand.CommandType);
            foreach (var flushType in flushTypes)
                engine.AddFlushHandler(flushType);
        }

        private object[] ProcessQuery(VisualBase visual, Event.VisualQueryArgs visualCommand)
        {
            Debug.Assert(visualCommand != null);
            var queryHandler = GalaxyEntry.GetModule<VisualProxyRegister>().GetQuery(visualCommand.QueryType);
            return queryHandler.Query(visual, visualCommand);
        }

        private object[] ProcessQuery(EngineProxy engine, Event.EngineQueryArgs engineCommand)
        {
            Debug.Assert(engineCommand != null);
            var queryHandler = GalaxyEntry.GetModule<VisualProxyRegister>().GetQuery(engineCommand.QueryType);
            return queryHandler.Query(engine, engineCommand);
        }
    }
}
