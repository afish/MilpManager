using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class ArraySetCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            return type == CompositeOperationType.ArraySet && arguments.Any() && parameters is ArraySetParameters &&
                   ((ArraySetParameters) parameters).Index.IsInteger() &&
                   ((ArraySetParameters) parameters).Index.IsNonNegative();
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            var index = ((ArraySetParameters) parameters).Index;
            var value = ((ArraySetParameters) parameters).Value;
            var catenatedArguments = string.Join(", ", arguments.Select(a => a.FullExpression()).ToArray());
            for (int i = 0; i < arguments.Length; ++i)
            {
                arguments[i] = milpManager.Operation(OperationType.Condition, milpManager.FromConstant(i).Operation(OperationType.IsEqual, index), value, arguments[i]);
                arguments[i].Expression = $"arraySet(wantedIndex: {index.FullExpression()}, value: {value.FullExpression()}, inArrayIndex: {i}, {catenatedArguments})";
            }
            
            return arguments;
        }
    }
}