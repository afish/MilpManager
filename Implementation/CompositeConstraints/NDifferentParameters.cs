using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class NDifferentParameters : ICompositeConstraintParameters
    {
        public NDifferentParameters(int valuesCount)
        {
            ValuesCount = valuesCount;
        }

        public int ValuesCount { get; private set; }
    }
}