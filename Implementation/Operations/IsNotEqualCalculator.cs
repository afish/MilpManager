using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class IsNotEqualCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var first = arguments[0];
			var second = arguments[1];

			var isGreater = first.Operation<IsGreaterThan>(second);
			var isLess = first.Operation<IsLessThan>(second);
			var disjunction = isGreater.Operation<Disjunction>(isLess);

			disjunction.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
				? arguments[0].ConstantValue != arguments[1].ConstantValue ? 1 : 0
				: (double?)null;
			SolverUtilities.SetExpression(disjunction, $"{arguments[0].FullExpression()} ?!= {arguments[1].FullExpression()}");
			return disjunction;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant(arguments[0].ConstantValue.Value != arguments[1].ConstantValue.Value ? 1 : 0);
		}

		protected override Type[] SupportedTypes => new[] {typeof (IsNotEqual)};
	}
}
