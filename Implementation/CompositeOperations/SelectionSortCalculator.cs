using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeOperations
{
	public class SelectionSortCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return arguments.Any();
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var results = milpManager.CompositeOperation<NthElements>(
				new NthElementsParameters { Indexes = Enumerable.Range(0, arguments.Length).Select(milpManager.FromConstant).ToArray() },
				arguments).ToArray();
			for (int i = 0; i < results.Length; ++i)
			{
				SolverUtilities.SetExpression(results[i], $"selectionSort(position: {i + 1}, {string.Join(",", arguments.Select(a => a.FullExpression()).ToArray())})");
			}
			return results;
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			return arguments.OrderBy(a => a.ConstantValue.Value);
		}

		protected override Type[] SupportedTypes => new[] {typeof (SelectionSort)};
	}
}