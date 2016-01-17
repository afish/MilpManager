using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class DisjunctionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Disjunction && arguments.Length >= 1 && arguments.All(a => a.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant(arguments.Select(a => (int)a.ConstantValue).Aggregate(Math.Max));
            }
            var variable = milpManager.CreateAnonymous(Domain.BinaryInteger);
            var sum = milpManager.Operation(OperationType.Addition, arguments);
            var argumentsCount = arguments.Length;
            sum.Operation(OperationType.Subtraction,
                milpManager.FromConstant(argumentsCount).Operation(OperationType.Multiplication, variable))
                .Set(ConstraintType.LessOrEqual, milpManager.FromConstant(0))
                .Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(-(argumentsCount - 1)));

            variable.Expression = $"{string.Join(" || ", arguments.Select(a => a.FullExpression()).ToArray())}";
            return variable;
        }
    }
}
