using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class ConditionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Condition &&  arguments.Length == 3 && arguments[0].IsBinary() && arguments[1].IsInteger() && arguments[2].IsInteger();
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            return milpManager.Create(milpManager.Operation(OperationType.Addition,
                arguments[0].Operation(OperationType.Multiplication, arguments[1]),
                arguments[0].Operation(OperationType.BinaryNegation)
                    .Operation(OperationType.Multiplication, arguments[2])
                ));
        }
    }
}
