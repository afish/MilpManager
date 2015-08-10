using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsLessOrEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsLessOrEqual && arguments.Length == 2;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            return arguments[0].Operation(OperationType.IsGreaterThan, arguments[1])
                .Operation(OperationType.BinaryNegation);
        }
    }
}
