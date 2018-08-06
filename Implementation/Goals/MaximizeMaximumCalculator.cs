using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Goals
{
	public class MaximizeMaximumCalculator : BaseGoalCalculator
	{
		protected override bool SupportsOperationInternal<TGoalType>(params IVariable[] arguments)
		{
			return arguments.Any();
		}

		protected override IVariable CalculateInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var result = milpManager.CreateAnonymous(arguments.Any(a => a.IsReal()) ? Domain.AnyReal : Domain.AnyInteger);
			arguments.Aggregate(milpManager.FromConstant(0), (existing, next) => existing.Operation<Disjunction>(result.Operation<IsEqual>(next)))
				.Set<Equal>(milpManager.FromConstant(1));

			return result;
		}

		protected override Type[] SupportedTypes => new[] {typeof (MaximizeMaximum)};

		protected override IVariable CalculateConstantInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.Operation<Maximum>(arguments);
		}
	}
}