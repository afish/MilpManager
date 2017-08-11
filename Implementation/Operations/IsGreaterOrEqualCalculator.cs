using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class IsGreaterOrEqualCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var result = arguments[0].Operation<IsLessThan>(arguments[1])
				.Operation<BinaryNegation>();

			result.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
				? arguments[0].ConstantValue >= arguments[1].ConstantValue ? 1 : 0
				: (double?)null;
			SolverUtilities.SetExpression(result, $"{arguments[0].FullExpression()} ?>= {arguments[1].FullExpression()}");
			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant(arguments[0].ConstantValue.Value >= arguments[1].ConstantValue.Value ? 1 : 0);
		}

		protected override Type[] SupportedTypes => new[] {typeof (IsGreaterOrEqual)};
	}
}
