using EngSim.Src.Interfaces;
using System;

namespace EngSim.Src.EngineTests
{
    internal class OverheatTimeTest : EngineTestBase, IObserver
    {
        public bool IsInfinite { get; private set; }
        public double ResultTime { get; private set; }

        public OverheatTimeTest( IEngine engine ) : base(engine) { }

        public override void ConnectToEngine()
        {
            if(_engine is IObservable observable && _engine is IOverheatTimeTestable)
            {
                observable.AddObserver(this);
            }
            else
            {
                throw new NotImplementedException("This type of engine isn't supported.");
            }
        }

        public void Update()
        {
            if(!_engine.IsRun)
            {
                var comEng = _engine as IOverheatTimeTestable;
                if(!comEng.IsOverheat)
                {
                    IsInfinite = true;
                    LogText = "Двигатель никогда не перегреется. Вечный двигатель успешно создан!";
                }
                else
                {
                    ResultTime = comEng.Time;
                    LogText = $"Двигатель перегрелся за {ResultTime:0.###} секунд.";
                }
                var observable = _engine as IObservable;
                observable.RemoveObserver(this);
            }
        }
    }
}
