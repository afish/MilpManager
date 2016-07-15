using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class ConjunctionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Conjunction && arguments.Length >= 1 && arguments.All(a => a.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant(arguments.Select(a => (int) a.ConstantValue).Aggregate(Math.Min));
            }
            var variable = milpManager.CreateAnonymous(Domain.BinaryInteger);
            var sum = milpManager.Operation(OperationType.Addition, arguments);
            var argumentsCount = arguments.Length;
            sum.Operation(OperationType.Subtraction,
                milpManager.FromConstant(argumentsCount).Operation(OperationType.Multiplication, variable))
                .Set(ConstraintType.LessOrEqual, milpManager.FromConstant(argumentsCount - 1))
                .Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(0));

            variable.ConstantValue = arguments.Aggregate((double?)1.0, (a, b) => a.HasValue && b.ConstantValue.HasValue ? Math.Min(a.Value, b.ConstantValue.Value) : (double?)null);
            variable.Expression = $"{string.Join(" && ", arguments.Select(a => a.FullExpression()).ToArray())}";
            return variable;
        }
    }
}
