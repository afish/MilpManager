using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeOperations
{
	public class ApproximateCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return arguments.Length == 1 && parameters is ApproximateParameters;
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = parameters as ApproximateParameters;
			var x = arguments.First();

			if (arguments.All(i => i.IsConstant()))
			{
				return new[] { milpManager.FromConstant(typedParameters.Function(x.ConstantValue.Value)) };
			}

			var one = milpManager.FromConstant(1);
			var points = typedParameters.Arguments.Select(a => Tuple.Create(milpManager.FromConstant(a), milpManager.FromConstant(typedParameters.Function(a)))).ToArray();
			var variables = points.Select(p => milpManager.CreateAnonymous(typedParameters.ArgumentMustBeOnAGrid ? Domain.BinaryInteger : Domain.PositiveOrZeroReal).Set<LessOrEqual>(one)).ToArray();

			x.Set<Equal>(milpManager.Operation<Addition>(points.Select((point, index) => variables[index].Operation<Multiplication>(point.Item1)).ToArray()));
			var y = milpManager.Operation<Addition>(points.Select((point, index) => variables[index].Operation<Multiplication>(point.Item2)).ToArray());

			milpManager.Operation<Addition>(variables).Set<Equal>(one);
			milpManager.Set<SpecialOrderedSetType2>(variables.First(), variables.Skip(1).ToArray());

			y.ConstantValue = x.ConstantValue.HasValue ? typedParameters.Function(x.ConstantValue.Value) : (double?)null;
			SolverUtilities.SetExpression(y, $"approximation({typedParameters.FunctionDescription})");

			return new[] { y };
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			return CalculateInternal<TCompositeOperationType>(milpManager, parameters, arguments);
		}

		protected override Type[] SupportedTypes => new[] {typeof (Approximate)};
	}
}