using EngSim.Src.Interfaces;

namespace EngSim.Src.EngineTests
{
    internal abstract class EngineTestBase
    {
        protected readonly IEngine _engine;

        public string LogText { get; protected set; }

        public EngineTestBase( IEngine engine )
        {
            _engine = engine;
        }

        public abstract void ConnectToEngine();
    }
}
