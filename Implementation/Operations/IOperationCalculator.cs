using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public interface IOperationCalculator
    {
        bool SupportsOperation(OperationType type, params IVariable[] arguments);

        IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments);
    }
}