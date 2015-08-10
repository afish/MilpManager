using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class RemainderCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Remainder && arguments.Length == 2 &&
                   arguments.All(a => a.IsPositiveOrZero() || a.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            IVariable numerator = arguments[0];
            IVariable denominator = arguments[1];

            IVariable result = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            result.Set(ConstraintType.LessOrEqual, denominator.Operation(OperationType.Subtraction, milpManager.FromConstant(1)));

            IVariable any = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            numerator.Set(ConstraintType.Equal,
                any.Operation(OperationType.Multiplication, denominator).Operation(OperationType.Addition, result));

            return result;
        }
    }
}