using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class EquivalencyCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Equivalency && arguments.Length >= 1 && arguments.All(a => a.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                var sum = arguments.Select(a => (int)a.ConstantValue).Sum();
                return milpManager.FromConstant(sum == arguments.Count() || sum == 0 ? 1 : 0);
            }

            var variable = milpManager.Operation(OperationType.Conjunction,
                arguments.Zip(arguments.Skip(1), (a,b) => 
                milpManager.Operation(OperationType.Disjunction,
                milpManager.Operation(OperationType.Conjunction, a, b),
                milpManager.Operation(OperationType.Conjunction, a.Operation(OperationType.BinaryNegation), b.Operation(OperationType.BinaryNegation)))).ToArray());
            variable.Expression = $"{string.Join(" <==> ", arguments.Select(a => a.FullExpression()).ToArray())}";
            return variable;
        }
    }
}
