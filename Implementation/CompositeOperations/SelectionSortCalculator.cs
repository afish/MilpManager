using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class SelectionSortCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            return type == CompositeOperationType.SelectionSort && arguments.All(a => a.IsInteger());
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] with parameters {parameters} not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                return arguments.OrderBy(a => a.ConstantValue.Value);
            }
            return milpManager.CompositeOperation(CompositeOperationType.NthElements,
                new NthElementsParameters {Indexes = Enumerable.Range(0, arguments.Length).ToArray()},
                arguments);
        }
    }
}