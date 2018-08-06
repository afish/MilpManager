using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeOperations
{
	public abstract class LexicographicalCompareCalculator : BaseCompositeOperationCalculator
	{
		private static IEnumerable<IVariable> CalculateBatches(IMilpManager milpManager, ICompositeOperationParameters parameters,
			IVariable[] arguments)
		{
			var batchSize = milpManager.IntegerWidth;
			var batches = (arguments.Length - 1)/batchSize + 1;
			return Enumerable.Range(0, batches)
				.Select(
					index =>
						CalculateBatch(milpManager, arguments.Skip(index*batchSize).Take(batchSize).ToArray(),
							(parameters as LexicographicalCompareParameters).Pattern.Skip(index*batchSize).Take(batchSize).ToArray()));
		}

		private static IVariable CalculateBatch(IMilpManager milpManager, IVariable[] first, IVariable[] second)
		{
			var power = (int)Math.Pow(2, first.Length);
			return first.Zip(second, Tuple.Create).Aggregate(milpManager.FromConstant(0),
				(result, pair) =>
				{
					power /= 2;
					return result.Operation<Addition>(
						pair.Item1.Operation<IsGreaterOrEqual>(pair.Item2).Operation<Multiplication>(milpManager.FromConstant(power)),
						pair.Item1.Operation<IsLessOrEqual>(pair.Item2).Operation<Multiplication>(milpManager.FromConstant(-power))
					);
				});
		}

		protected abstract IVariable CompareFinalResult(IVariable result, IMilpManager milpManager);

		protected abstract int ConstantFinalResult(int result);
		protected abstract string ComparerFinalResult { get; }

		protected override bool IsConstantOperation<TCompositeOperationType>(ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = parameters as LexicographicalCompareParameters;
			return base.IsConstantOperation<TCompositeOperationType>(parameters, arguments) &&
			       typedParameters.Pattern.All(i => i.IsConstant());
		}

		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return (parameters as LexicographicalCompareParameters)?.Pattern?.Length == arguments.Length;
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = parameters as LexicographicalCompareParameters;
			var compareResult = CalculateBatches(milpManager, parameters, arguments).ToArray();
			while (compareResult.Length > 1)
			{
				var zero = milpManager.FromConstant(0);
				var newParameters = new LexicographicalCompareParameters
				{
					Pattern = Enumerable.Range(0, compareResult.Length).Select(x => zero).ToArray()
				};
				compareResult = CalculateBatches(milpManager, newParameters, compareResult).ToArray();
			}

			var result = compareResult.First();
			result.ConstantValue = arguments.All(a => a.ConstantValue.HasValue) && typedParameters.Pattern.All(a => a.ConstantValue.HasValue)
				? ConstantFinalResult(arguments.Zip(typedParameters.Pattern, Tuple.Create).Select(p => p.Item1.ConstantValue.Value - p.Item2.ConstantValue.Value).Select(v => v > 0 ? 1 : v < 0 ? -1 : 0).TakeWhile(v => v != 0).FirstOrDefault())
				: (double?)null;
			SolverUtilities.SetExpression(result,
				$"({string.Join(",", arguments.Select(a => a.FullExpression()).ToArray())}) {ComparerFinalResult} ({string.Join(",", typedParameters.Pattern.Select(a => a.FullExpression()).ToArray())})");

			return new[] { CompareFinalResult(compareResult[0], milpManager) };
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = parameters as LexicographicalCompareParameters;
			return new[] {
					CompareFinalResult(milpManager.FromConstant(arguments.Zip(typedParameters.Pattern, Tuple.Create)
						.Select(pair => pair.Item1.ConstantValue.Value - pair.Item2.ConstantValue.Value)
						.Select(v => v > 0 ? 1 : (v < 0 ? -1 : 0))
						.FirstOrDefault(v => v != 0)), milpManager)
				};
		}
	}
}