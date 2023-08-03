using EngSim.Src.Interfaces;
using System;

namespace EngSim.Src.EngineTests
{
    internal class MaxPowerTest : EngineTestBase, IObserver
    {
        private double _maxPowerLast = double.MinValue;
        private double _crankshaftRotSpeed;

        public MaxPowerTest( IEngine engine ) : base(engine) { }

        public override void ConnectToEngine()
        {
            if(_engine is IObservable observable && _engine is IMaxPowerTestable)
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
            var comEng = _engine as IMaxPowerTestable;
            if(_maxPowerLast >= comEng.PowerMax) // если двигатель перестал раскручиваться
            {
                var observable =  _engine as IObservable;
                observable.RemoveObserver(this);
                LogText = $"Максимальная достигнутая мощность равна {_maxPowerLast:0.###} кВт.\n" +
                    $"Данное значение достигнуто при скорости вращения коленвала = {_crankshaftRotSpeed:0.###} радиан/сек.";
            }
            else
            {
                _maxPowerLast = comEng.PowerMax;
                _crankshaftRotSpeed = comEng.VNow;
            }
        }
    }
}
