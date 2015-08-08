using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class IsEqualCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.IsEqual && arguments.Length == 2;
        }

        public IVariable Calculate(BaseMilpManager baseMilpManager, OperationType type, params IVariable[] arguments)
        {
            return baseMilpManager.Operation(OperationType.IsNotEqual, arguments).Operation(OperationType.BinaryNegation);
        }
    }
}
