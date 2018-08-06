using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Operations
{
	public class ConjunctionCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length >= 1 && arguments.All(a => a.IsBinary());
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var variable = milpManager.CreateAnonymous(Domain.BinaryInteger);
		    variable.ConstantValue = arguments.Aggregate((double?)1.0, (a, b) => a.HasValue && b.ConstantValue.HasValue ? Math.Min(a.Value, b.ConstantValue.Value) : (double?)null);

            var sum = milpManager.Operation<Addition>(arguments);
			var argumentsCount = arguments.Length;
			sum.Operation<Subtraction>(milpManager.FromConstant(argumentsCount).Operation<Multiplication>(variable))
				.Set<LessOrEqual>(milpManager.FromConstant(argumentsCount - 1))
				.Set<GreaterOrEqual>(milpManager.FromConstant(0));

			SolverUtilities.SetExpression(variable, $"{string.Join(" && ", arguments.Select(a => a.FullExpression()).ToArray())}");
			return variable;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant(arguments.Select(a => (int)a.ConstantValue).Aggregate(Math.Min));
		}

		protected override Type[] SupportedTypes => new[] {typeof (Conjunction)};
	}
}
