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

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            return milpManager.CompositeOperation(CompositeOperationType.NthElements,
                new NthElementsParameters {Indexes = Enumerable.Range(0, arguments.Length).ToArray()},
                arguments);
        }
    }
}