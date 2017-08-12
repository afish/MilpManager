using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
	public class MinimizeMaximumCalculator : BaseGoalCalculator
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
				result.Set<GreaterOrEqual>(argument);
			}

			return result.MakeGoal<Minimize>();
		}

		protected override Type[] SupportedTypes => new[] {typeof (MinimizeMaximum)};
		protected override IVariable CalculateConstantInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.Operation<Maximum>(arguments);
		}
	}
}