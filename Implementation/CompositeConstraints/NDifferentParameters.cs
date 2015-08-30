using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class NDifferentParameters : ICompositeConstraintParameters
    {
        public NDifferentParameters(int valuesCount, Domain preferredDomain)
        {
            ValuesCount = valuesCount;
            PreferredDomain = preferredDomain;
        }

        public int ValuesCount { get; private set; }
        public Domain PreferredDomain { get; private set; }
    }
}