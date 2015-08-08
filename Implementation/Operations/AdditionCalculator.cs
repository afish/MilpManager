using System.Collections.Generic;
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

        public IVariable Calculate(BaseMilpManager baseMilpManager, OperationType type, params IVariable[] arguments)
        {
            var domain = CalculateDomain(arguments);

            return arguments.Aggregate((x,y) => baseMilpManager.SumVariables(x, y, domain));
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
