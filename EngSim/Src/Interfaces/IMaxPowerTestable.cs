namespace EngSim.Src.Interfaces
{
    internal interface IMaxPowerTestable
    {
        double PowerMax { get; }

        double VNow { get; } // скорость вращения коленвала
    }
}
