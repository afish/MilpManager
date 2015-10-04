using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class BinaryNegationCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.BinaryNegation && arguments.Length == 1 && arguments.All(a => a.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments[0].IsConstant())
            {
                return milpManager.FromConstant((int) (1 - arguments[0].ConstantValue.Value));
            }
            return milpManager.FromConstant(1).Operation(OperationType.Subtraction, arguments[0]).ChangeDomain(Domain.BinaryInteger).Create();
        }
    }
}
