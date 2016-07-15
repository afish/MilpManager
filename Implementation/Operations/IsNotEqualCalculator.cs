using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsNotEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsNotEqual && arguments.Length == 2 && (arguments.All(x => x.IsInteger()) || arguments.All(x => x.IsConstant()));
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant(arguments[0].ConstantValue.Value != arguments[1].ConstantValue.Value ? 1 : 0);
            }
            var first = arguments[0];
            var second = arguments[1];

            var isGreater = first.Operation(OperationType.IsGreaterThan, second);
            var isLess = first.Operation(OperationType.IsLessThan, second);
            var disjunction = isGreater.Operation(OperationType.Disjunction, isLess);

            disjunction.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
                ? arguments[0].ConstantValue != arguments[1].ConstantValue ? 1 : 0
                : (double?)null;
            disjunction.Expression = $"{arguments[0].FullExpression()} ?!= {arguments[1].FullExpression()}";
            return disjunction;
        }
    }
}
