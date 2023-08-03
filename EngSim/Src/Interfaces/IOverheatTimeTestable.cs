namespace EngSim.Src.Interfaces
{
    internal interface IOverheatTimeTestable
    {
        bool IsOverheat { get; }
        double Time { get; }
    }
}
