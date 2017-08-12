using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class LoopCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return (parameters as LoopParameters)?.Body?.Length == arguments.Length;
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var options = parameters as LoopParameters;

			var totalBound = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
			totalBound.Set<LessOrEqual>(milpManager.FromConstant(options.MaxIterations));

			options.BeforeLoopAction(totalBound, arguments);

			for (int i = 1; i <= options.MaxIterations; ++i)
			{
				var counter = milpManager.FromConstant(i);
				var isLooping = counter.Operation<IsLessOrEqual>(totalBound);

				options.BeforeIterationAction(counter, isLooping, totalBound, arguments);

				for (int v = 0; v < arguments.Length; ++v)
				{
					if (options.BeforeBody.Length > v)
					{
						options.BeforeBody[v](arguments[v], counter, isLooping, totalBound, arguments);
					}

					arguments[v] = milpManager.Operation<Condition>(isLooping, options.Body[v](arguments[v], counter, isLooping, totalBound, arguments), arguments[v]);

					if (options.AfterBody.Length > v)
					{
						options.AfterBody[v](arguments[v], counter, isLooping, totalBound, arguments);
					}
				}

				options.AfterIterationAction(counter, isLooping, totalBound, arguments);
			}

			options.AfterLoopAction(totalBound, arguments);

			return arguments.Concat(new[] { totalBound }).ToArray();
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			return CalculateInternal<TCompositeOperationType>(milpManager, parameters, arguments);
		}

		protected override Type[] SupportedTypes => new[] {typeof (Loop)};
	}
}