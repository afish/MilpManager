using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsNotEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsNotEqual && arguments.Length == 2 && arguments.All(x => x.IsInteger()); ;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            var first = arguments[0];
            var second = arguments[1];

            var isGreater = first.Operation(OperationType.IsGreaterThan, second);
            var isLess = first.Operation(OperationType.IsLessThan, second);
            var disjunction = isGreater.Operation(OperationType.Disjunction, isLess);

            return disjunction;
        }
    }
}
