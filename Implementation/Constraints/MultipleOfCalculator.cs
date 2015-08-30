using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
    internal class MultipleOfCalculator : IConstraintCalculator
    {
        public IVariable Set(IMilpManager baseMilpManager, ConstraintType type, IVariable leftVariable, IVariable rightVariable)
        {
            IVariable any = baseMilpManager.CreateAnonymous(Domain.AnyInteger);
            leftVariable.Set(ConstraintType.Equal,any.Operation(OperationType.Multiplication, rightVariable));

            return leftVariable;
        }
    }
}
