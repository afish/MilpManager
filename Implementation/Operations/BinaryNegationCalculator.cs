using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class BinaryNegationCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 1 && arguments.All(a => a.IsBinary());
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var result = milpManager.FromConstant(1).Operation<Subtraction>(arguments[0]).ChangeDomain(Domain.BinaryInteger).Create();
			result.Expression = $"!{arguments[0].FullExpression()}";
			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant((int)(1 - arguments[0].ConstantValue.Value));
		}

		protected override Type[] SupportedTypes => new[] {typeof (BinaryNegation)};
	}
}
