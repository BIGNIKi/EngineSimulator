using EngSim.Src.Engines;

namespace EngSim.Src.ClassCreators
{
    internal class EngineCreators
    {
        public static CombustionEngine CreateCombustionEngine()
        {
            double momentInertiaEngine = 10; // кг*м^2                                 I
            double[] torques = { 20, 75, 100, 105, 75, 0 }; // Н*м                     M
            double[] crankshaftRotSpeed = { 0, 75, 150, 200, 250, 300 }; // радиан/сек V
            double overheatingTemperature = 110; // °C                                 T_перегрева
            double coeffDepHeatSpeedTorque = 0.01; // °C / (Н * m * сек)               Н_м
            double coeffDepHeatSpeedRotationCrankshaft =  0.0001; // °C * сек / рад^2  H_v
            double coeffDepCoolingRateTempeEngineEnv = 0.1; // 1 / сек                 C

            return new(momentInertiaEngine, torques, crankshaftRotSpeed,
                overheatingTemperature, coeffDepHeatSpeedTorque,
                coeffDepHeatSpeedRotationCrankshaft,
                coeffDepCoolingRateTempeEngineEnv);
        }
    }
}
