using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeOperations
{
	public class ArrayGetCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return arguments.Any() && parameters is ArrayGetParameters &&
				   ((ArrayGetParameters)parameters).Index.IsInteger() &&
				   ((ArrayGetParameters)parameters).Index.IsNonNegative();
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = (ArrayGetParameters)parameters;
			if (typedParameters.Index.IsConstant())
			{
				return new[] { arguments[(int)typedParameters.Index.ConstantValue.Value] };
			}

			var index = typedParameters.Index;
			var result = milpManager.CreateAnonymous(arguments.Skip(1)
					.Aggregate(arguments[0].Domain, (domain, next) => domain.LowestEncompassingDomain(next.Domain)));

		    result.ConstantValue = index.ConstantValue.HasValue ? arguments[(int)index.ConstantValue.Value].ConstantValue : null;

            for (int i = 0; i < arguments.Length; ++i)
			{
				milpManager.FromConstant(i).Operation<IsEqual>(index)
					.Operation<MaterialImplication>(result.Operation<IsEqual>(arguments[i]))
					.MakeTrue();
			}

			SolverUtilities.SetExpression(result, $"arrayGet(index: {index.FullExpression()}, {string.Join(", ", arguments.Select(a => a.FullExpression()).ToArray())})");

			return new[] { result };
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			return CalculateInternal<TCompositeOperationType>(milpManager, parameters, arguments);
		}

		protected override Type[] SupportedTypes => new[] {typeof (ArrayGet)};
	}
}