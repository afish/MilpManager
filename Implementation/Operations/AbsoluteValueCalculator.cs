using System;
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
            if(!SupportsOperation(type,arguments))throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");

            var number = arguments[0];
            var numberNegated = number.Operation(OperationType.Negation);
            var result = milpManager.CreateAnonymous(number.IsInteger() ? Domain.PositiveOrZeroInteger : Domain.PositiveOrZeroReal);

            result.Set(ConstraintType.GreaterOrEqual, number)
                .Set(ConstraintType.GreaterOrEqual, numberNegated);

            milpManager.Operation(OperationType.Addition,
                    result.Operation(OperationType.IsEqual, number),
                    result.Operation(OperationType.IsEqual, numberNegated))
                .Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(1));

            return result;
        }
    }
}
