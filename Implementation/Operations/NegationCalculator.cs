﻿using System;
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
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
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

            var result = milpManager.NegateVariable(arguments[0], domain);
            result.ConstantValue = -arguments[0].ConstantValue;
            result.Expression = $"-{arguments[0].FullExpression()}";
            return result;
        }
    }
}
