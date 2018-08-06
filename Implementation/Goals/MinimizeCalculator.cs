using System;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Goals
{
	public class MinimizeCalculator : BaseGoalCalculator
	{
		protected override bool SupportsOperationInternal<TGoalType>(params IVariable[] arguments)
		{
			return arguments.Length == 1;
		}

		protected override IVariable CalculateInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return arguments[0].Operation<Negation>();
		}

		protected override Type[] SupportedTypes => new[] {typeof (Minimize)};
	}
}