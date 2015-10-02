using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
    public class NDifferentCalculator : ICompositeConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, CompositeConstraintType type, ICompositeConstraintParameters parameters,
            IVariable leftVariable, params IVariable[] rightVariable)
        {
            leftVariable.Operation(OperationType.DifferentValuesCount, rightVariable)
                .Set(ConstraintType.Equal, milpManager.FromConstant((parameters as NDifferentParameters).ValuesCount));

            return leftVariable;
        }
    }
}