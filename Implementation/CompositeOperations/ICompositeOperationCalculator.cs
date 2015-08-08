using System.Collections.Generic;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public interface ICompositeOperationCalculator
    {
        bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments);

        IEnumerable<IVariable> Calculate(BaseMilpManager baseMilpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments);
    }
}