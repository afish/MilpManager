using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class ArraySetCalculator : ICompositeOperationCalculator
	{
		public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return type == CompositeOperationType.ArraySet && arguments.Any() && parameters is ArraySetParameters &&
				   ((ArraySetParameters) parameters).Index.IsInteger() &&
				   ((ArraySetParameters) parameters).Index.IsNonNegative();
		}

		public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, parameters, arguments));
			var index = ((ArraySetParameters) parameters).Index;
			var value = ((ArraySetParameters)parameters).Value;
			if (index.IsConstant())
			{
				arguments[(int) index.ConstantValue.Value] = value;
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
	}
}