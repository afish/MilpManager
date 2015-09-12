using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsGreaterThanCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsGreaterThan && arguments.Length == 2 && arguments.All(x => x.IsInteger());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            var result = milpManager.CreateAnonymous(Domain.BinaryInteger);

            var first = arguments[0];
            var second = arguments[1];

            second.Operation(OperationType.Subtraction, first)
                .Operation(OperationType.Addition,
                    result.Operation(OperationType.Multiplication, milpManager.FromConstant(milpManager.IntegerInfinity)))
                .Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(0))
                .Set(ConstraintType.LessOrEqual, milpManager.FromConstant(milpManager.IntegerInfinity - 1));

            return result;
        }
    }
}
