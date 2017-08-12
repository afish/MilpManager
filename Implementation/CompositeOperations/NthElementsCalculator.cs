using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class NthElementsCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return parameters is NthElementsParameters &&
				   ((NthElementsParameters)parameters).Indexes.All(
					   i => i.IsInteger() && (i.IsBinary() || i.IsPositiveOrZero()));
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = parameters as NthElementsParameters;
			var variables = new List<IVariable>();
			var sums = arguments.Select(a => Tuple.Create(a, milpManager.Operation<Addition>(
				arguments.Where(b => a != b).Select(b => a.Operation<IsGreaterOrEqual>(b).Create()).ToArray()).Create())).ToArray();

			var huge = milpManager.FromConstant(milpManager.MaximumIntegerValue);
			foreach (var indexVariable in typedParameters.Indexes)
			{
				var result = huge;
				foreach (var sum in sums)
				{
					result = result.Operation<Minimum>(milpManager.Operation<Condition>(
						sum.Item2.Operation<IsGreaterOrEqual>(indexVariable),
						sum.Item1,
						huge
					));
				}


				var singleVariable = result.Create();
				SolverUtilities.SetExpression(singleVariable, $"nthElement(index: {indexVariable.FullExpression()}, {string.Join(",", arguments.Select(a => a.FullExpression()).ToArray())})");
				variables.Add(singleVariable);
			}

			return variables;
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = parameters as NthElementsParameters;
			var sorted = arguments.OrderBy(a => a.ConstantValue.Value).ToArray();
			return typedParameters.Indexes.Select(i => sorted[(int)i.ConstantValue.Value]);
		}

		protected override bool IsConstantOperation<TCompositeOperationType>(ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = parameters as NthElementsParameters;
			return base.IsConstantOperation<TCompositeOperationType>(parameters, arguments) &&
			       typedParameters.Indexes.All(a => a.IsConstant());
		}

		protected override Type[] SupportedTypes => new[] {typeof (NthElements)};
	}
}