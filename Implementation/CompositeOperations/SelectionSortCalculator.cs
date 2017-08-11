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
			return type == CompositeOperationType.SelectionSort && arguments.Any();
		}

		public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, parameters, arguments));
			if (arguments.All(a => a.IsConstant()))
			{
				return arguments.OrderBy(a => a.ConstantValue.Value);
			}
			var results = milpManager.CompositeOperation(CompositeOperationType.NthElements,
				new NthElementsParameters {Indexes = Enumerable.Range(0, arguments.Length).Select(milpManager.FromConstant).ToArray()},
				arguments).ToArray();
			for (int i = 0; i < results.Length; ++i)
			{
				SolverUtilities.SetExpression(results[i], $"selectionSort(position: {i+1}, {string.Join(",", arguments.Select(a => a.FullExpression()).ToArray())})");
			}
			return results;
		}
	}
}