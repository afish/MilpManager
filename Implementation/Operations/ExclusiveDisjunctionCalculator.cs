using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class ExclusiveDisjunctionCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length >= 1 && arguments.All(a => a.IsBinary());
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var variable = arguments.Aggregate((a, b) =>
				milpManager.Operation<Disjunction>(
				milpManager.Operation<Conjunction>(a.Operation<BinaryNegation>(), b),
				milpManager.Operation<Conjunction>(a, b.Operation<BinaryNegation>())));
			variable.Expression = $"{string.Join(" ^ ", arguments.Select(a => a.FullExpression()).ToArray())}";
			return variable;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant(arguments.Select(a => (int)a.ConstantValue).Aggregate((a, b) => (a + b) % 2));
		}

		protected override Type[] SupportedTypes => new[] {typeof (ExclusiveDisjunction)};
	}
}
