using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class RemainderCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Remainder && arguments.Length == 2 && arguments.All(x => x.IsPositiveOrZero() || x.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            IVariable numerator = arguments[0];
            IVariable denominator = arguments[1];

            var one = milpManager.FromConstant(1);
            var any = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            any.Operation(OperationType.Multiplication, denominator).Set(ConstraintType.LessOrEqual, numerator);
            any.Operation(OperationType.Addition, one)
                .Operation(OperationType.Multiplication, denominator)
                .Set(ConstraintType.GreaterOrEqual, numerator.Operation(OperationType.Addition, one));
            
            IVariable result = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            result.Set(ConstraintType.LessOrEqual, denominator);
            result.Set(ConstraintType.Equal,
                numerator.Operation(OperationType.Subtraction,
                    denominator.Operation(OperationType.Multiplication, any)));

            return result;
        }
    }
}