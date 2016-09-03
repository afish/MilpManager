using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class ArrayGetCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            return type == CompositeOperationType.ArrayGet && arguments.Any() && parameters is ArrayGetParameters &&
                   ((ArrayGetParameters) parameters).Index.IsInteger() &&
                   ((ArrayGetParameters) parameters).Index.IsNonNegative();
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            var index = ((ArrayGetParameters) parameters).Index;
            var result = milpManager.FromConstant(0);
            for (int i = 0; i < arguments.Length; ++i)
            {
                result = milpManager.Operation(OperationType.Condition, milpManager.FromConstant(i).Operation(OperationType.IsEqual, index), arguments[i], result);
            }

            result.ConstantValue = index.ConstantValue.HasValue ? arguments[(int)index.ConstantValue.Value].ConstantValue : null;
            result.Expression = $"arrayGet(index: {index.FullExpression()}, {string.Join(", ", arguments.Select(a => a.FullExpression()).ToArray())})";

            return new[] {result};
        }
    }
}