using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class EquivalencyCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length >= 1 && arguments.All(a => a.IsBinary());
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var variable = milpManager.Operation<Conjunction>(arguments.Zip(arguments.Skip(1), (a, b) =>
				milpManager.Operation<Disjunction>(
				milpManager.Operation<Conjunction>(a, b),
				milpManager.Operation<Conjunction>(a.Operation<BinaryNegation>(), b.Operation<BinaryNegation>()))).ToArray());
			SolverUtilities.SetExpression(variable, $"{string.Join(" <==> ", arguments.Select(a => a.FullExpression()).ToArray())}");
			return variable;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var sum = arguments.Select(a => (int)a.ConstantValue).Sum();
			return milpManager.FromConstant(sum == arguments.Count() || sum == 0 ? 1 : 0);
		}

		protected override Type[] SupportedTypes => new[] {typeof (Equivalency)};
	}
}
