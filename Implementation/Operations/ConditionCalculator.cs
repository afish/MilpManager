using System;
using System.Linq;
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
                if (arguments[0].ConstantValue.Value <= milpManager.Epsilon)
                {
                    return arguments[2];
                }
                return arguments[1];
            }
            var trueBranch = arguments[0].Operation(OperationType.Multiplication, arguments[1]);
            var falseBranch = arguments[0].Operation(OperationType.BinaryNegation)
                .Operation(OperationType.Multiplication, arguments[2]);
            var result = milpManager.Create(milpManager.Operation(OperationType.Addition,
                trueBranch,
                falseBranch
                ).ChangeDomain(CalculateDomain(trueBranch, falseBranch)));
            result.Expression = $"{arguments[0].FullExpression()} ? {arguments[1].FullExpression()} : {arguments[2].FullExpression()}";
            result.ConstantValue = !arguments[0].ConstantValue.HasValue
                ? null
                : (int) arguments[0].ConstantValue.Value == 1 ? trueBranch.ConstantValue : falseBranch.ConstantValue;
            return result;
        }

        private static Domain CalculateDomain(IVariable first, IVariable second)
        {
            if (first.IsBinary())
            {
                return second.Domain.MakeNonConstant();
            }
            if (second.IsBinary())
            {
                return first.Domain.MakeNonConstant();
            }

            if (first.IsInteger() && second.IsInteger())
            {
                if (first.IsPositiveOrZero() && second.IsPositiveOrZero())
                {
                    return Domain.PositiveOrZeroInteger;
                }
                else
                {
                    return Domain.AnyInteger;
                }
            }

            if (first.IsPositiveOrZero() && second.IsPositiveOrZero())
            {
                return Domain.PositiveOrZeroReal;
            }

            return Domain.AnyReal;
        }
    }
}
