using EngSim.Src.DTO;
using EngSim.Src.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EngSim.Src.Engines
{
    internal class CombustionEngine : IEngine, IObservable, IOverheatTimeTestable, IMaxPowerTestable
    {
        private double _I; // Момент инерции двигателя
        private double[] _M; // Кусочно-линейная зависимость крутящего момента
        private double[] _V; // Скорость вращения коленвала
        private double _T; // Температура перегрева
        private double _Hm; // Коэффициент зависимости скорости нагрева от крутящего момента 
        private double _Hv; // Коэффициент зависимости скорости нагрева от скорости вращения коленвала
        private double _C; // Коэффициент зависимости скорости охлаждения от температуры двигателя и окружающей среды

        private readonly List<IObserver> _observers = new();

        public double AmbientTemp { get; set; } = 0;
        public double Accuracy { get; set; } = 1;

        public bool IsRun { get; private set; } = false;

        public bool IsOverheat { get; private set; } = false;
        public double Time { get; private set; }

        public double PowerMax { get; private set; } = double.MinValue; // мощность кВт
        public double VNow { get; private set; } // скорость вращения коленвала

        public CombustionEngine( double momentInertiaEngine,
            double[] torques, double[] crankshaftRotSpeed,
            double overheatingTemperature,
            double coeffDepHeatSpeedTorque,
            double coeffDepHeatSpeedRotationCrankshaft,
            double coeffDepCoolingRateTempeEngineEnv )
        {
            _I = momentInertiaEngine;
            _M = torques; 
            _V = crankshaftRotSpeed;
            if(_M.Length != _V.Length)
            {
                throw new ArgumentException("Length of the arrays \"crankshaftRotSpeed\" and \"torques\" must be equals.");
            }
            if(_M.Length == 0)
            {
                throw new ArgumentException("Function parameters must be set.");
            }
            _T = overheatingTemperature;
            _Hm = coeffDepHeatSpeedTorque;
            _Hv = coeffDepHeatSpeedRotationCrankshaft;
            _C = coeffDepCoolingRateTempeEngineEnv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ambientTemp"></param>
        /// <param name="accuracy"></param>
        /// <param name="time"></param>
        /// <returns>true - infinite time, false - finite time</returns>
        public void Start()
        {
            IsRun = true;

            double tempNow = AmbientTemp; // температура сейчас
            VNow = _V[0]; // скорость вращения коленвала сейчас
            Time = 0;
            var linesCoeff = CalculateLinesParameters();
            while(tempNow < _T)
            {
                double mNow = GetTorqueNow(VNow, linesCoeff); // крутящий момент сейчас
                double acc = mNow / _I;
                double vh = mNow * _Hm + Math.Pow(VNow, 2) * _Hv; // скорость нагрева
                double vc = _C * (AmbientTemp - tempNow); // скорость охлаждения

                var temp = tempNow;
                tempNow += (vh - vc) / Accuracy;
                VNow += acc / Accuracy;
                PowerMax = mNow * VNow / 1000;
                Time += 1f / Accuracy;
                if(tempNow.Equals(temp)) // если нет динамики температуры, считаем, что двигатель никогда не перегреется
                {
                    IsRun = false;
                    NotifyObservers();
                    return;
                }
                NotifyObservers();
            }

            IsRun = false;
            IsOverheat = true;
            NotifyObservers();
        }

        private double GetTorqueNow( double crankshaftRotSpeedNow, List<LineParameters> linesCoeff )
        {
            int idCRS = _V.Length-1;
            while(crankshaftRotSpeedNow < _V[idCRS])
                idCRS--;

            if(idCRS == _V.Length - 1)
            {
                throw new ArgumentException($"Values of torque for V > {_V[_V.Length-1]} is not provided");
            }

            return linesCoeff[idCRS].K * crankshaftRotSpeedNow + linesCoeff[idCRS].B;
        }

        private List<LineParameters> CalculateLinesParameters()
        {
            var linesCoeff = new List<LineParameters>();
            for(int i = 0; i < _M.Length-1; i++)
            {
                double k = (_M[i+1] - _M[i]) / (_V[i+1] - _V[i]);
                double b = _M[i] - k * _V[i];
                var @new = new LineParameters(k, b);
                linesCoeff.Add(@new);
            }

            return linesCoeff;
        }

        public void AddObserver( IObserver observer )
        {
            _observers.Add(observer);
        }

        public void RemoveObserver( IObserver observer )
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach(var item in _observers.ToList())
                item.Update();
        }
    }
}
