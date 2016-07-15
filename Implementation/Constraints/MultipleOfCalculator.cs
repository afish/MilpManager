using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
    public class MultipleOfCalculator : IConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, ConstraintType type, IVariable leftVariable, IVariable rightVariable)
        {
            IVariable any = milpManager.CreateAnonymous(Domain.AnyInteger);
            leftVariable.Set(ConstraintType.Equal,any.Operation(OperationType.Multiplication, rightVariable));

            return leftVariable;
        }
    }
}
