using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
	public class MinimizeMinimumCalculator : BaseGoalCalculator
	{
		protected override bool SupportsOperationInternal<TGoalType>(params IVariable[] arguments)
		{
			return arguments.Any();
		}

		protected override IVariable CalculateInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.MakeGoal<MaximizeMaximum>(arguments).MakeGoal<Minimize>();
		}

		protected override Type[] SupportedTypes => new[] {typeof (MinimizeMinimum)};

		protected override IVariable CalculateConstantInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.Operation<Minimum>(arguments);
		}
	}
}