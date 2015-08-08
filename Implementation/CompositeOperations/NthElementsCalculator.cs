using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    internal class NthElementsCalculator : ICompositeOperationCalculator
    {
        public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            return type == CompositeOperationType.NthElements && arguments.All(a => a.IsInteger()) && parameters is NthElementsParameters;
        }

        public IEnumerable<IVariable> Calculate(BaseMilpManager baseMilpManager, CompositeOperationType type,
            ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            var typedParameters = parameters as NthElementsParameters;
            var variables = new List<IVariable>();
            var sums = arguments.Select(a => Tuple.Create(a, baseMilpManager.Operation(OperationType.Addition,
                arguments.Where(b => a != b).Select(b => a.Operation(OperationType.IsGreaterOrEqual, b).Create()).ToArray()).Create())).ToArray();

            var huge = baseMilpManager.FromConstant(baseMilpManager.MaximumIntegerValue);
            foreach(var index in typedParameters.Indexes)
            {
                var result = huge;
                IVariable indexVariable = baseMilpManager.FromConstant(index);
                foreach (var sum in sums)
                {
                    result = result.Operation(OperationType.Minimum, baseMilpManager.Operation(OperationType.Condition,
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