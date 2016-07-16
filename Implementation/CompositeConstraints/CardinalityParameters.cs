using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class CardinalityParameters : ICompositeConstraintParameters
    {
        public CardinalityParameters(int valuesCount)
        {
            ValuesCount = valuesCount;
        }

        public int ValuesCount { get; private set; }
    }
}