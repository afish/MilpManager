using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class ExclusiveDisjunctionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.ExclusiveDisjunction && arguments.Length >= 1 && arguments.All(a => a.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant(arguments.Select(a => (int)a.ConstantValue).Aggregate((a, b) => (a + b) % 2));
            }

            var variable = arguments.Aggregate((a,b) => 
                milpManager.Operation(OperationType.Disjunction,
                milpManager.Operation(OperationType.Conjunction, a.Operation(OperationType.BinaryNegation), b),
                milpManager.Operation(OperationType.Conjunction, a, b.Operation(OperationType.BinaryNegation))));
            variable.Expression = $"{string.Join(" ^ ", arguments.Select(a => a.FullExpression()).ToArray())}";
            return variable;
        }
    }
}
