using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    internal class SortCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            return type == CompositeOperationType.Sort && arguments.All(a => a.IsInteger());
        }

        public IEnumerable<IVariable> Calculate(BaseMilpManager baseMilpManager, CompositeOperationType type, ICompositeOperationParameters parameters, 
            params IVariable[] arguments)
        {
            return baseMilpManager.CompositeOperation(CompositeOperationType.NthElements,
                new NthElementsParameters {Indexes = Enumerable.Range(0, arguments.Length).ToArray()},
                arguments);
        }
    }
}