namespace EngSim.Src.DTO
{
    internal record LineParameters
    {
        public double K { get; private set; }

        public double B { get; private set; }

        public LineParameters( double k , double b) 
        {
            K = k;
            B = b;
        }
    }
}
