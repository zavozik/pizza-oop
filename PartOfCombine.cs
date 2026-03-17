namespace ConsoleApp2
{
    internal partial class Program
    {
        class PartOfCombine
        {
            public IHasPrice Pizza { get; private set; }
            public int Pieces { get; private set; }

            public PartOfCombine(IHasPrice pizza, int pieces)
            {
                Pizza = pizza;
                Pieces = pieces;
            }
        }
    }
}