using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class ConditionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Condition && arguments.Length == 3 && arguments[0].IsBinary() &&
                   ((arguments[1].IsInteger() && arguments[2].IsInteger()) || arguments[0].IsConstant());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments[0].IsConstant())
            {
                if (arguments[0].ConstantValue.Value <= 0.00001)
                {
                    return arguments[2];
                }
                return arguments[1];
            }
            var result = milpManager.Create(milpManager.Operation(OperationType.Addition,
                arguments[0].Operation(OperationType.Multiplication, arguments[1]),
                arguments[0].Operation(OperationType.BinaryNegation)
                    .Operation(OperationType.Multiplication, arguments[2])
                ));
            result.Expression = $"{arguments[0].FullExpression()} ? {arguments[1].FullExpression()} : {arguments[2].FullExpression()}";
            return result;
        }
    }
}
