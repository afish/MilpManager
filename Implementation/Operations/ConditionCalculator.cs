using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class ConditionCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 3 && arguments[0].IsBinary() &&
				   ((arguments[1].IsInteger() && arguments[2].IsInteger()) || arguments[0].IsConstant());
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var trueBranch = arguments[0].Operation<Multiplication>(arguments[1]);
			var falseBranch = arguments[0].Operation<BinaryNegation>()
				.Operation<Multiplication>(arguments[2]);
			var result = milpManager.Create(milpManager.Operation<Addition>(
				trueBranch,
				falseBranch
				).ChangeDomain(trueBranch.LowestEncompassingDomain(falseBranch)));
			result.Expression = $"{arguments[0].FullExpression()} ? {arguments[1].FullExpression()} : {arguments[2].FullExpression()}";
			result.ConstantValue = !arguments[0].ConstantValue.HasValue
				? null
				: (int)arguments[0].ConstantValue.Value == 1 ? trueBranch.ConstantValue : falseBranch.ConstantValue;
			return result;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			if (arguments[0].ConstantValue.Value <= milpManager.Epsilon)
			{
				return arguments[2];
			}
			return arguments[1];
		}

		protected override Type[] SupportedTypes => new[] {typeof (Condition)};
	}
}
