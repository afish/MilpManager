using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Goals
{
	public class MaximizeCalculator : BaseGoalCalculator
	{
		protected override bool SupportsOperationInternal<TGoalType>(params IVariable[] arguments)
		{
			return arguments.Length == 1;
		}

		protected override IVariable CalculateInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return arguments[0];
		}

		protected override Type[] SupportedTypes => new[] {typeof (Maximize)};
	}
}