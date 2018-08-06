using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Goals
{
	public abstract class BaseGoalCalculator : IGoalCalculator
	{
		public bool SupportsOperation<TGoalType>(params IVariable[] arguments) where TGoalType : Goal
		{
			return SupportedTypes.Contains(typeof(TGoalType)) && SupportsOperationInternal<TGoalType>(arguments);
		}

		public IVariable Calculate<TGoalType>(IMilpManager milpManager, params IVariable[] arguments) where TGoalType : Goal
		{
			if (!SupportsOperation<TGoalType>(arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(typeof(TGoalType), arguments));

			if (arguments.All(x => x.IsConstant()))
			{
				return CalculateConstantInternal<TGoalType>(milpManager, arguments);
			}
			else
			{
				var result = CalculateInternal<TGoalType>(milpManager, arguments);

				return result;
			}
		}

		protected virtual IVariable CalculateConstantInternal<TGoalType>(IMilpManager milpManager,
			params IVariable[] arguments)
			where TGoalType : Goal
		{
			return CalculateInternal<TGoalType>(milpManager, arguments);
		}

		protected abstract bool SupportsOperationInternal<TGoalType>(params IVariable[] arguments)
			where TGoalType : Goal;

		protected abstract IVariable CalculateInternal<TGoalType>(IMilpManager milpManager, params IVariable[] arguments)
			where TGoalType : Goal;

		protected abstract Type[] SupportedTypes { get; }
	}
}