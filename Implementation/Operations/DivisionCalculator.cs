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
                   (IsPhysicalDivision(arguments) || arguments.All(a => (a.IsPositiveOrZero() || a.IsBinary()) && a.IsInteger()));
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            var domain = CalculateDomain(arguments);
            if (IsPhysicalDivision(arguments))
            {
                var finalDomain = arguments.All(x => x.IsConstant()) ? domain.MakeConstant() : domain;
                return milpManager.DivideVariableByConstant(arguments[0], arguments[1], finalDomain);
            }

            IVariable one = milpManager.FromConstant(1);
            var result = milpManager.CreateAnonymous(domain);
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
                return Domain.BinaryInteger;
            }

            if (IsPhysicalDivision(arguments))
            {
                if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
                {
                    return Domain.PositiveOrZeroReal;
                }
                return Domain.AnyReal;
            }

            if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
            {
                return Domain.PositiveOrZeroInteger;
            }
            return Domain.AnyInteger;
        }

        private static bool IsPhysicalDivision(IVariable[] arguments)
        {
            return arguments[1].IsConstant();
        }
    }
}
