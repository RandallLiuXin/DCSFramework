using Galaxy.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Visual.Query
{
    public abstract class VisualQuery : QueryBase
    {
        public abstract VisualQueryType GetVisualQueryType();

        protected override object[] QueryPrepared(EngineProxy engine, Event.EngineQueryArgs eventArgs)
        {
            return null;
        }

        protected override object[] QueryNotPrepared(EngineProxy engine, Event.EngineQueryArgs eventArgs)
        {
            return null;
        }
    }

    public class VisualQueryPosition : VisualQuery
    {
        public override VisualQueryType GetVisualQueryType()
        {
            return VisualQueryType.Position;
        }

        protected override object[] QueryNotPrepared(VisualBase visual, VisualQueryArgs eventArgs)
        {
            //throw new GalaxyException("VisualQueryPosition QueryNotPrepared!");
            return null;
        }

        protected override object[] QueryPrepared(VisualBase visual, VisualQueryArgs eventArgs)
        {
            return new object[] { visual.GetPosition() };
        }
    }

    public class VisualQueryRotation : VisualQuery
    {
        public override VisualQueryType GetVisualQueryType()
        {
            return VisualQueryType.Rotation;
        }

        protected override object[] QueryNotPrepared(VisualBase visual, VisualQueryArgs eventArgs)
        {
            //throw new GalaxyException("VisualQueryRotation QueryNotPrepared!");
            return null;
        }

        protected override object[] QueryPrepared(VisualBase visual, VisualQueryArgs eventArgs)
        {
            return new object[] { visual.GetRotation() };
        }
    }

    public class VisualQueryGetSkeleton : VisualQuery
    {
        public override VisualQueryType GetVisualQueryType()
        {
            return VisualQueryType.GetSkeleton;
        }

        protected override object[] QueryNotPrepared(VisualBase visual, VisualQueryArgs eventArgs)
        {
            //throw new GalaxyException("VisualQueryRotation QueryNotPrepared!");
            return null;
        }

        protected override object[] QueryPrepared(VisualBase visual, VisualQueryArgs eventArgs)
        {
            ModelProxy model = visual as ModelProxy;
            Debug.Assert(model != null);
            return new object[] { model.GetSkeleton() };
        }
    }
}
