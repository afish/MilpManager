using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class ArraySetCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return arguments.Any() && parameters is ArraySetParameters &&
				   ((ArraySetParameters)parameters).Index.IsInteger() &&
				   ((ArraySetParameters)parameters).Index.IsNonNegative();
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var index = ((ArraySetParameters)parameters).Index;
			var value = ((ArraySetParameters)parameters).Value;
			if (index.IsConstant())
			{
				arguments[(int)index.ConstantValue.Value] = value;
				return arguments;
			}

			var catenatedArguments = string.Join(", ", arguments.Select(a => a.FullExpression()).ToArray());
			for (int i = 0; i < arguments.Length; ++i)
			{
				arguments[i] = milpManager.Operation<Condition>(milpManager.FromConstant(i).Operation<IsEqual>(index), value, arguments[i]);
				SolverUtilities.SetExpression(arguments[i], $"arraySet(wantedIndex: {index.FullExpression()}, value: {value.FullExpression()}, inArrayIndex: {i}, {catenatedArguments})");
			}

			return arguments;
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			return CalculateInternal<TCompositeOperationType>(milpManager, parameters, arguments);
		}

		protected override Type[] SupportedTypes => new[] {typeof (ArraySet)};
	}
}