using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
	public class MaximizeMinimumCalculator : BaseGoalCalculator
	{
		protected override bool SupportsOperationInternal<TGoalType>(params IVariable[] arguments)
		{
			return arguments.Any();
		}

		protected override IVariable CalculateInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var result = milpManager.CreateAnonymous(arguments.Any(a => a.IsReal()) ? Domain.AnyReal : Domain.AnyInteger);
			foreach (var argument in arguments)
			{
				result.Set<LessOrEqual>(argument);
			}

			return result;
		}

		protected override Type[] SupportedTypes => new[] {typeof (MaximizeMinimum) };

		protected override IVariable CalculateConstantInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.Operation<Minimum>(arguments);
		}
	}
}