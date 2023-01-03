using UnityEngine;

namespace Galaxy.Visual.Query
{
    public abstract class EngineQuery : QueryBase
    {
        public abstract EngineQueryType GetEngineQueryType();

        protected override object[] QueryPrepared(VisualBase visual, Event.VisualQueryArgs eventArgs)
        {
            return null;
        }

        protected override object[] QueryNotPrepared(VisualBase visual, Event.VisualQueryArgs eventArgs)
        {
            return null;
        }
    }

    public class EngineQueryGetScene : EngineQuery
    {
        public override EngineQueryType GetEngineQueryType()
        {
            return EngineQueryType.GetScene;
        }

        protected override object[] QueryNotPrepared(EngineProxy engine, Event.EngineQueryArgs eventArgs)
        {
            //throw new GalaxyException("VisualQueryPosition QueryNotPrepared!");
            return null;
        }

        protected override object[] QueryPrepared(EngineProxy engine, Event.EngineQueryArgs eventArgs)
        {
            return new object[] { engine.GetScene() };
        }
    }
}
