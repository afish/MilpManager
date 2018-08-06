using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeOperations
{
	public class CountingSortCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return arguments.Any() && parameters is CountingSortParameters;
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var castedParameters = parameters as CountingSortParameters;
			var values = castedParameters.Values;
			var valuesWithCounts = new Dictionary<IVariable, IVariable>();
			var zero = milpManager.FromConstant(0);
			foreach (var value in values)
			{
				valuesWithCounts[value] = arguments.Aggregate(zero,
					(current, val) =>
						current.Operation<Addition>(val.Operation<IsEqual>(value)));
			}

			var sum = zero;
			foreach (var value in values)
			{
				sum = sum.Operation<Addition>(valuesWithCounts[value]);
				valuesWithCounts[value] = sum;
			}

			var infinity = milpManager.FromConstant(milpManager.MaximumIntegerValue);
			var results = Enumerable.Range(1, arguments.Length).Select(p =>
			{
				var position = milpManager.FromConstant(p);
				var result = milpManager.Operation<Minimum>(
					values.Select(value =>
						milpManager.Operation<Condition>(
							position.Operation<IsLessOrEqual>(valuesWithCounts[value]), value, infinity)
						).ToArray());
				SolverUtilities.SetExpression(result, $"countingSort(position: {p}, {string.Join(", ", arguments.Select(a => a.FullExpression()).ToArray())})");
				return result;
			}).ToArray();

			return results;
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			return arguments.OrderBy(a => a.ConstantValue.Value);
		}

		protected override Type[] SupportedTypes => new[] {typeof (CountingSort)};
	}
}