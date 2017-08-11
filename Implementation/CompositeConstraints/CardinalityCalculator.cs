using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class CardinalityCalculator : ICompositeConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters,
            IVariable leftVariable, params IVariable[] rightVariable)
        {
            leftVariable.Operation<DifferentValuesCount>(rightVariable)
                .Set<Equal>(milpManager.FromConstant((parameters as CardinalityParameters).ValuesCount));

            return leftVariable;
        }
    }
}