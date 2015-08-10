using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsLessThanCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsLessThan && arguments.Length == 2;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            return milpManager.Operation(OperationType.IsGreaterThan, arguments[1], arguments[0]);
        }
    }
}
