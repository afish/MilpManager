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

        public IVariable Calculate(BaseMilpManager baseMilpManager, OperationType type, params IVariable[] arguments)
        {
            var number = arguments[0];
            var result = baseMilpManager.CreateAnonymous(number.IsInteger() ? Domain.PositiveOrZeroInteger : Domain.PositiveOrZeroReal);

            result.Set(ConstraintType.GreaterOrEqual, number)
                .Set(ConstraintType.GreaterOrEqual, number.Operation(OperationType.Negation));

            baseMilpManager.Operation(OperationType.Disjunction,
                    result.Operation(OperationType.Addition, number).Operation(OperationType.IsEqual, baseMilpManager.FromConstant(0)),
                    result.Operation(OperationType.Subtraction, number).Operation(OperationType.IsEqual, baseMilpManager.FromConstant(0)))
                .Set(ConstraintType.Equal, baseMilpManager.FromConstant(1));

            return result;
        }
    }
}
