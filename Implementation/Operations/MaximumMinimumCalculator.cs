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

        public IVariable Calculate(BaseMilpManager baseMilpManager, OperationType type, params IVariable[] arguments)
        {
            var first = arguments[0];
            var second = arguments[1];

            IVariable max = baseMilpManager.CreateAnonymous(CalculateDomain(arguments));
            IVariable min = baseMilpManager.CreateAnonymous(CalculateDomain(arguments));

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
            return arguments.All(a => a.IsInteger()) ? Domain.AnyInteger : Domain.AnyReal;
        }
    }
}
