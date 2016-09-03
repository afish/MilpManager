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
            return type == CompositeOperationType.NthElements &&
                   parameters is NthElementsParameters &&
                   ((NthElementsParameters) parameters).Indexes.All(
                       i => i.IsInteger() && (i.IsBinary() || i.IsPositiveOrZero()));
        }

        public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, parameters, arguments));
            var typedParameters = parameters as NthElementsParameters;
            if (arguments.All(a => a.IsConstant()) && typedParameters.Indexes.All(a => a.IsConstant()))
            {
                var sorted = arguments.OrderBy(a => a.ConstantValue.Value).ToArray();
                return typedParameters.Indexes.Select(i => sorted[(int)i.ConstantValue.Value]);
            }
            var variables = new List<IVariable>();
            var sums = arguments.Select(a => Tuple.Create(a, milpManager.Operation(OperationType.Addition,
                arguments.Where(b => a != b).Select(b => a.Operation(OperationType.IsGreaterOrEqual, b).Create()).ToArray()).Create())).ToArray();

            var huge = milpManager.FromConstant(milpManager.MaximumIntegerValue);
            foreach(var indexVariable in typedParameters.Indexes)
            {
                var result = huge;
                foreach (var sum in sums)
                {
                    result = result.Operation(OperationType.Minimum, milpManager.Operation(OperationType.Condition,
                        sum.Item2.Operation(OperationType.IsGreaterOrEqual, indexVariable),
                        sum.Item1,
                        huge
                    ));
                }


                var singleVariable = result.Create();
                singleVariable.Expression = $"nthElement(index: {indexVariable.FullExpression()}, {string.Join(",", arguments.Select(a => a.FullExpression()).ToArray())})";
                variables.Add(singleVariable);
            }

            return variables;
        }
    }
}