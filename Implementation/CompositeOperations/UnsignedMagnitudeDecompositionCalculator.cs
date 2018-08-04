using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class UnsignedMagnitudeDecompositionCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
		    return arguments.Length == 1 && arguments[0].IsNonNegative();
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var decomposition = CalculateDecomposition(milpManager, arguments);

			return decomposition.Select((v, index) =>
			{
				SolverUtilities.SetExpression(v, $"unsignedMagnitudeDecomposition(bit: {index}, {arguments[0].FullExpression()})");
				return v;
			});
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			return CalculateDecomposition(milpManager, arguments);
		}

		private static IEnumerable<IVariable> CalculateDecomposition(IMilpManager milpManager, IVariable[] arguments)
		{
			return milpManager.CompositeOperation<Decomposition>(new DecompositionParameters { Base = 2 }, arguments);
		}

		protected override Type[] SupportedTypes => new[] {typeof (UnsignedMagnitudeDecomposition)};
	}
}