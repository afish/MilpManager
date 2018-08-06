using System.Collections.Generic;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public interface ICompositeOperationCalculator
    {
        bool SupportsOperation<TCompositeOperationType>(ICompositeOperationParameters parameters, params IVariable[] arguments) where TCompositeOperationType : CompositeOperation;

        IEnumerable<IVariable> Calculate<TCompositeOperationType>(IMilpManager milpManager, ICompositeOperationParameters parameters, params IVariable[] arguments) where TCompositeOperationType : CompositeOperation;
    }
}