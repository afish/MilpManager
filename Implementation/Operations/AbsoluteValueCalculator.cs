using System.Runtime.CompilerServices;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class AbsoluteValueCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.AbsoluteValue && arguments.Length == 1;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            var number = arguments[0];
            var result = milpManager.CreateAnonymous(number.IsInteger() ? Domain.PositiveOrZeroInteger : Domain.PositiveOrZeroReal);

            result.Set(ConstraintType.GreaterOrEqual, number)
                .Set(ConstraintType.GreaterOrEqual, number.Operation(OperationType.Negation));

            milpManager.Operation(OperationType.Disjunction,
                    result.Operation(OperationType.Addition, number).Operation(OperationType.IsEqual, milpManager.FromConstant(0)),
                    result.Operation(OperationType.Subtraction, number).Operation(OperationType.IsEqual, milpManager.FromConstant(0)))
                .Set(ConstraintType.Equal, milpManager.FromConstant(1));

            return result;
        }
    }
}
