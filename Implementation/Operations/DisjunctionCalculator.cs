using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class DisjunctionCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length >= 1 && arguments.All(a => a.IsBinary());
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var variable = milpManager.CreateAnonymous(Domain.BinaryInteger);
			var sum = milpManager.Operation<Addition>(arguments);
			var argumentsCount = arguments.Length;
			sum.Operation<Subtraction>(milpManager.FromConstant(argumentsCount).Operation<Multiplication>(variable))
				.Set<LessOrEqual>(milpManager.FromConstant(0))
				.Set<GreaterOrEqual>(milpManager.FromConstant(-(argumentsCount - 1)));

			variable.ConstantValue = arguments.Aggregate((double?)0.0, (a, b) => a.HasValue && b.ConstantValue.HasValue ? Math.Max(a.Value, b.ConstantValue.Value) : (double?)null);
			SolverUtilities.SetExpression(variable, $"{string.Join(" || ", arguments.Select(a => a.FullExpression()).ToArray())}");
			return variable;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant(arguments.Select(a => (int)a.ConstantValue).Aggregate(Math.Max));
		}

		protected override Type[] SupportedTypes => new[] {typeof (Disjunction)};
	}
}
