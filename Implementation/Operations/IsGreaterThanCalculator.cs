using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsGreaterThanCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsGreaterThan && arguments.Length == 2 && (arguments.All(x => x.IsInteger()) || arguments.All(x => x.IsConstant()));
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant(arguments[0].ConstantValue.Value > arguments[1].ConstantValue.Value ? 1 : 0);
            }
            var result = milpManager.CreateAnonymous(Domain.BinaryInteger);

            var first = arguments[0];
            var second = arguments[1];

            second.Operation(OperationType.Subtraction, first)
                .Operation(OperationType.Addition,
                    result.Operation(OperationType.Multiplication, milpManager.FromConstant(milpManager.IntegerInfinity)))
                .Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(0))
                .Set(ConstraintType.LessOrEqual, milpManager.FromConstant(milpManager.IntegerInfinity - 1));

            result.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
                ? arguments[0].ConstantValue > arguments[1].ConstantValue ? 1 : 0
                : (double?)null;
            result.Expression = $"{arguments[0].FullExpression()} ?> {arguments[1].FullExpression()}";
            return result;
        }
    }
}
