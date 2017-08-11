using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class DifferentValuesCountCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length > 0;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var total = milpManager.FromConstant(0);
			foreach (var first in arguments)
			{
				var different = milpManager.FromConstant(1);
				foreach (var second in arguments.TakeWhile(a => a != first))
				{
					different = different.Operation<Conjunction>(first.Operation<IsNotEqual>(second));
				}
				total = total.Operation<Addition>(different);
			}

			SolverUtilities.SetExpression(total, $"differentValuesCount({string.Join(",", arguments.Select(a => a.FullExpression()).ToArray())})");
			return total;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant(arguments.Select(a => a.ConstantValue.Value).Distinct().Count());
		}

		protected override Type[] SupportedTypes => new[] {typeof (DifferentValuesCount)};
	}
}
