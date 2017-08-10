using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
	public class SubtractionCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return arguments.Aggregate((left, right) => left.Operation<Addition>(right.Operation<Negation>()));
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return CalculateInternal<TOperationType>(milpManager, arguments);
		}

		protected override Type[] SupportedTypes => new[] {typeof (Subtraction)};
	}
}
