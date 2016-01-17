using System;
using System.Linq;
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
            if(!SupportsOperation(type,arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(x => x.IsConstant()))
            {
                if (arguments[0].IsInteger())
                {
                    return milpManager.FromConstant(Math.Abs((int)arguments[0].ConstantValue.Value));
                }
                return milpManager.FromConstant(Math.Abs(arguments[0].ConstantValue.Value));
            }

            var number = arguments[0];
            var numberNegated = number.Operation(OperationType.Negation);
            var result = milpManager.CreateAnonymous(number.IsInteger() ? Domain.PositiveOrZeroInteger : Domain.PositiveOrZeroReal);

            result.Set(ConstraintType.GreaterOrEqual, number)
                .Set(ConstraintType.GreaterOrEqual, numberNegated);

            milpManager.Operation(OperationType.Addition,
                    result.Operation(OperationType.IsEqual, number),
                    result.Operation(OperationType.IsEqual, numberNegated))
                .Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(1));

            result.Expression = $"|{arguments[0].FullExpression()}|";
            return result;
        }
    }
}
