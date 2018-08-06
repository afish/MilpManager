using System;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Operations
{
	public class NegationCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 1;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			Domain domain = arguments[0].IsReal() ? Domain.AnyReal : Domain.AnyInteger;

			var result = milpManager.NegateVariable(arguments[0], domain);
			result.ConstantValue = -arguments[0].ConstantValue;
			SolverUtilities.SetExpression(result, $"-{arguments[0].FullExpression()}");
			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var value = -arguments[0].ConstantValue.Value;
			if (arguments[0].IsInteger())
			{
				return milpManager.FromConstant((int)value);
			}
			else
			{
				return milpManager.FromConstant(value);
			}
		}

		protected override Type[] SupportedTypes => new[] {typeof (Negation)};
	}
}
