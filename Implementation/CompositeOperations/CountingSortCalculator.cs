using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    internal class CountingSortCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            return type == CompositeOperationType.CountingSort && arguments.All(a => a.IsInteger());
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            var castedParameters = parameters as CountingSortParameters;
            var values = castedParameters.Values.OrderBy(x => x).Select(milpManager.FromConstant).ToArray();
            var valuesWithCounts = new Dictionary<IVariable, IVariable>();
            var zero = milpManager.FromConstant(0);
            foreach (var value in values)
            {
                valuesWithCounts[value] = arguments.Aggregate(zero,
                    (current, val) =>
                        current.Operation(OperationType.Addition, val.Operation(OperationType.IsEqual, value)));
            }
            
            var sum = zero;
            foreach (var value in values)
            {
                sum = sum.Operation(OperationType.Addition, valuesWithCounts[value]);
                valuesWithCounts[value] = sum;
            }

            var infinity = milpManager.FromConstant(milpManager.MaximumIntegerValue);
            var results = Enumerable.Range(1, arguments.Length).Select(p =>
            {
                var position = milpManager.FromConstant(p);
                return milpManager.Operation(OperationType.Minimum,
                    values.Select(value =>
                        milpManager.Operation(OperationType.Condition,
                            position.Operation(OperationType.IsLessOrEqual, valuesWithCounts[value]), value, infinity)
                        ).ToArray());
            }).ToArray();

            return results;
        }
    }
}