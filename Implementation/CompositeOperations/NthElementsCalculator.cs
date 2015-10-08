using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class NthElementsCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            return type == CompositeOperationType.NthElements && arguments.All(a => a.IsInteger()) && parameters is NthElementsParameters;
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] with parameters {parameters} not supported");
            var typedParameters = parameters as NthElementsParameters;
            if (arguments.All(a => a.IsConstant()))
            {
                var sorted = arguments.OrderBy(a => a.ConstantValue.Value).ToArray();
                return typedParameters.Indexes.Select(i => sorted[i]);
            }
            var variables = new List<IVariable>();
            var sums = arguments.Select(a => Tuple.Create(a, milpManager.Operation(OperationType.Addition,
                arguments.Where(b => a != b).Select(b => a.Operation(OperationType.IsGreaterOrEqual, b).Create()).ToArray()).Create())).ToArray();

            var huge = milpManager.FromConstant(milpManager.MaximumIntegerValue);
            foreach(var index in typedParameters.Indexes)
            {
                var result = huge;
                IVariable indexVariable = milpManager.FromConstant(index);
                foreach (var sum in sums)
                {
                    result = result.Operation(OperationType.Minimum, milpManager.Operation(OperationType.Condition,
                        sum.Item2.Operation(OperationType.IsGreaterOrEqual, indexVariable),
                        sum.Item1,
                        huge
                    ));
                }
                variables.Add(result.Create());
            }

            return variables;
        }
    }
}