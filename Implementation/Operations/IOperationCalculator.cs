using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public interface IOperationCalculator
    {
        bool SupportsOperation<TOperationType>(params IVariable[] arguments) where TOperationType : Operation;

        IVariable Calculate<TOperationType>(IMilpManager milpManager, params IVariable[] arguments) where TOperationType : Operation;
    }
}