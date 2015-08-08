using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsGreaterThanCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsGreaterThan && arguments.Length == 2;
        }

        public IVariable Calculate(BaseMilpManager baseMilpManager, OperationType type, params IVariable[] arguments)
        {
            var result = baseMilpManager.CreateAnonymous(Domain.BinaryInteger);

            var first = arguments[0];
            var second = arguments[1];

            second.Operation(OperationType.Subtraction, first)
                .Operation(OperationType.Addition,
                    result.Operation(OperationType.Multiplication, baseMilpManager.FromConstant(baseMilpManager.IntegerInfinity)))
                .Set(ConstraintType.GreaterOrEqual, baseMilpManager.FromConstant(0))
                .Set(ConstraintType.LessOrEqual, baseMilpManager.FromConstant(baseMilpManager.IntegerInfinity - 1));

            return result;
        }
    }
}
