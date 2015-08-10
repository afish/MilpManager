using System.Linq;
using MilpManager.Abstraction;
using Domain = MilpManager.Abstraction.Domain;

namespace MilpManager.Implementation.Operations
{
    public class DivisionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Division && arguments.Length == 2 &&
                   (arguments.Count(a => a.IsNotConstant()) <= 1 ||
                    (arguments.All(a => a.IsPositiveOrZero()) && arguments.All(a => a.IsInteger())));
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (arguments.Count(a => a.IsNotConstant()) <= 1)
            {
                var domain = CalculateDomain(arguments);
                return arguments.Aggregate((x, y) => milpManager.DivideVariableByConstant(x, y, domain));
            }

            IVariable one = milpManager.FromConstant(1);
            var result = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            result.Operation(OperationType.Multiplication, arguments[1])
                .Set(ConstraintType.LessOrEqual, arguments[0]);
            result.Operation(OperationType.Addition, one)
                .Operation(OperationType.Multiplication, arguments[1])
                .Set(ConstraintType.GreaterOrEqual, arguments[0].Operation(OperationType.Addition, one));

            return result;
        }

        private Domain CalculateDomain(IVariable[] arguments)
        {
            if (arguments.All(a => a.IsBinary()))
            {
                return arguments.Any(a => a.IsNotConstant()) ? Domain.BinaryInteger : Domain.BinaryConstantInteger;
            }

            var isRealDivision = arguments.Count(a => a.IsNotConstant()) <= 1;

            if (arguments.All(a => a.IsPositiveOrZero()))
            {
                if (arguments.Any(a => a.IsReal()) || isRealDivision)
                {
                    return arguments.Any(a => a.IsNotConstant()) ? Domain.PositiveOrZeroReal : Domain.PositiveOrZeroConstantReal;
                }

                return arguments.Any(a => a.IsNotConstant()) ? Domain.PositiveOrZeroInteger : Domain.PositiveOrZeroConstantInteger;
            }

            if (arguments.Any(a => a.IsReal()) || isRealDivision)
            {
                return arguments.Any(a => a.IsNotConstant()) ? Domain.AnyReal : Domain.AnyConstantReal;
            }

            return arguments.Any(a => a.IsNotConstant()) ? Domain.AnyInteger : Domain.AnyConstantInteger;
        }
    }
}
