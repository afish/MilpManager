using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class DecompositionCalculator : BaseCompositeOperationCalculator
	{
		private static IEnumerable<IVariable> CalculateForVariable(IMilpManager milpManager, IVariable[] arguments, uint decompositionBase)
		{
			List<Tuple<IVariable, int>> variables =
				Enumerable.Range(0, GetDigitsCount(milpManager, decompositionBase))
					.Select(i =>
					{
						var baseRaised = (int)Math.Pow(decompositionBase, i);
						var variable = milpManager.CreateAnonymous(decompositionBase == 2 ? Domain.BinaryInteger : Domain.PositiveOrZeroInteger);
						if (decompositionBase > 2)
						{
							variable = variable.Set<LessOrEqual>(milpManager.FromConstant((int) decompositionBase - 1));
						}
						return Tuple.Create(variable, baseRaised);
					})
					.ToList();

			milpManager.Operation<Addition>(
				variables.Select(v => v.Item1.Operation<Multiplication>(milpManager.FromConstant(v.Item2)))
					.ToArray()).Set<Equal>(arguments[0]);

			return variables.Select((v, index) => {
				var result = v.Item1;
				SolverUtilities.SetExpression(result, $"decomposition(digit: {index}, base: {decompositionBase}, {arguments[0].FullExpression()})");
				return result;
			});
		}

		private static int GetDigitsCount(IMilpManager milpManager, uint decompositionBase)
		{
			double value = 1;
			int digits = 0;
			while (value <= milpManager.MaximumIntegerValue)
			{
				digits++;
				value *= decompositionBase;
			}

			return digits;
		}

		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return parameters is DecompositionParameters &&
				   ((DecompositionParameters)parameters).Base >= 2 && arguments.Length == 1 && arguments[0].IsInteger() && arguments[0].IsNonNegative();
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var decompositionBase = ((DecompositionParameters)parameters).Base;
			foreach (var i in CalculateForVariable(milpManager, arguments, decompositionBase))
			{
				yield return i;
			}
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var decompositionBase = ((DecompositionParameters) parameters).Base;
			uint currentValue = (uint)arguments[0].ConstantValue.Value;
			for (int i = 0; i < GetDigitsCount(milpManager, decompositionBase); ++i)
			{
				yield return milpManager.FromConstant((int)(currentValue % decompositionBase));
				currentValue /= decompositionBase;
			}
			yield break;
		}

		protected override Type[] SupportedTypes => new[] {typeof (Decomposition)};
	}
}