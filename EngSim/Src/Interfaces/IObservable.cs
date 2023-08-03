namespace EngSim.Src.Interfaces
{
    internal interface IObservable
    {
        void AddObserver(IObserver observer);

        void RemoveObserver(IObserver observer);

        void NotifyObservers();
    }
}
