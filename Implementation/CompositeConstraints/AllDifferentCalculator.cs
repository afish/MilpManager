using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class AllDifferentCalculator : ICompositeConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters,
            IVariable leftVariable, params IVariable[] rightVariable)
        {
            leftVariable.Operation<DifferentValuesCount>(rightVariable)
                .Set(ConstraintType.Equal, milpManager.FromConstant(rightVariable.Length + 1));

            return leftVariable;
        }
    }
}