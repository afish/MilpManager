using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class NegationCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Negation && arguments.Length == 1;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments[0].IsConstant())
            {
                var value = -arguments[0].ConstantValue.Value;
                if (arguments[0].IsInteger())
                {
                    return milpManager.FromConstant((int) value);
                }
                else
                {
                    return milpManager.FromConstant(value);
                }
            }
            Domain domain = arguments[0].IsReal() ? Domain.AnyReal : Domain.AnyInteger;
            domain = arguments[0].IsConstant() ? domain.MakeConstant() : domain;

            var result = milpManager.NegateVariable(arguments[0], domain);
            result.ConstantValue = -arguments[0].ConstantValue;
            result.Expression = $"-{arguments[0].FullExpression()}";
            return result;
        }
    }
}
