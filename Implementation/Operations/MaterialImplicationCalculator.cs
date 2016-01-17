using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class MaterialImplicationCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.MaterialImplication && arguments.Length == 2 && arguments.All(a => a.IsBinary());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                return milpManager.FromConstant((int)(arguments[0].ConstantValue.Value == 0 ? 1 : arguments[1].ConstantValue.Value));
            }

            var variable = arguments[0].Operation(OperationType.BinaryNegation).Operation(OperationType.Disjunction, arguments[1]);
            variable.Expression = $"{arguments[0].FullExpression()} => {arguments[1].FullExpression()}";
            return variable;
        }
    }
}
