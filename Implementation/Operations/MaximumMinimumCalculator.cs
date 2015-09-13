using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class MaximumMinimumCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return (type == OperationType.Maximum || type == OperationType.Minimum) && arguments.Length >= 2;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.Length > 2)
            {
                return arguments[0].Operation(type, milpManager.Operation(type, arguments.Skip(1).ToArray()));
            }
            return CalculateForTwoVariables(milpManager, type, arguments);
        }

        private IVariable CalculateForTwoVariables(IMilpManager milpManager, OperationType type, IVariable[] arguments)
        {
            var first = arguments[0];
            var second = arguments[1];

            IVariable max = milpManager.CreateAnonymous(CalculateDomain(arguments));
            IVariable min = milpManager.CreateAnonymous(CalculateDomain(arguments));

            max.Set(ConstraintType.GreaterOrEqual, first);
            max.Set(ConstraintType.GreaterOrEqual, second);
            min.Set(ConstraintType.LessOrEqual, first);
            min.Set(ConstraintType.LessOrEqual, second);

            max.Operation(OperationType.Subtraction, min)
                .Set(ConstraintType.Equal,
                    first.Operation(OperationType.Subtraction, second).Operation(OperationType.AbsoluteValue));

            return type == OperationType.Maximum ? max : min;
        }

        private Domain CalculateDomain(params IVariable[] arguments)
        {
            if (arguments.Any(a => a.IsReal()))
            {
                if (arguments.All(a => a.IsPositiveOrZero()))
                {
                    return Domain.PositiveOrZeroReal;
                }
                return Domain.AnyReal;
            }

            if (arguments.All(a => a.IsBinary()))
            {
                return Domain.BinaryInteger;
            }

            if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
            {
                return Domain.PositiveOrZeroInteger;
            }

            return Domain.AnyInteger;
        }
    }
}
