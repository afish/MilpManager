using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsNotEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsNotEqual && arguments.Length == 2;
        }

        public IVariable Calculate(BaseMilpManager baseMilpManager, OperationType type, params IVariable[] arguments)
        {
            var first = arguments[0];
            var second = arguments[1];

            var isGreater = first.Operation(OperationType.IsGreaterThan, second);
            var isLess = first.Operation(OperationType.IsLessThan, second);
            var disjunction = isGreater.Operation(OperationType.Disjunction, isLess);

            return disjunction;
        }
    }
}
