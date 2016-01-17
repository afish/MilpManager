using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsLessOrEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsLessOrEqual && arguments.Length == 2 && (arguments.All(x => x.IsInteger()) || arguments.All(x => x.IsConstant()));
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant(arguments[0].ConstantValue.Value <= arguments[1].ConstantValue.Value ? 1 : 0);
            }
            var result = arguments[0].Operation(OperationType.IsGreaterThan, arguments[1])
                .Operation(OperationType.BinaryNegation);

            result.Expression = $"{arguments[0].FullExpression()} ?<= {arguments[1].FullExpression()}";
            return result;
        }
    }
}
