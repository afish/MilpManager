using System;
using System.Linq;
using MilpManager.Abstraction;
using Domain = MilpManager.Abstraction.Domain;

namespace MilpManager.Implementation.Operations
{
    public class AdditionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Addition && arguments.Length > 0;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            var domain = CalculateDomain(arguments);

            return arguments.Aggregate((x, y) =>
            {
                var result = milpManager.SumVariables(x, y, domain);
                result.ConstantValue = x.ConstantValue + y.ConstantValue;
                result.Expression = $"({x.Expression} + {y.Expression})";
                return result;
            });
        }

        private Domain CalculateDomain(IVariable[] arguments)
        {
            if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
            {
                if (arguments.Any(a => a.IsReal()))
                {
                    return arguments.Any(a => a.IsNotConstant()) ? Domain.PositiveOrZeroReal : Domain.PositiveOrZeroConstantReal;
                }

                return arguments.Any(a => a.IsNotConstant()) ? Domain.PositiveOrZeroInteger : Domain.PositiveOrZeroConstantInteger;
            }

            if (arguments.Any(a => a.IsReal()))
            {
                return arguments.Any(a => a.IsNotConstant()) ? Domain.AnyReal : Domain.AnyConstantReal;
            }

            return arguments.Any(a => a.IsNotConstant()) ? Domain.AnyInteger : Domain.AnyConstantInteger;
        }
    }
}
