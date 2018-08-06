using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeOperations
{
	public class Approximate2DCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return arguments.Length == 2 && parameters is Approximate2DParameters;
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = parameters as Approximate2DParameters;
			var x = arguments.First();
			var y = arguments.Skip(1).First();

			if (arguments.All(i => i.IsConstant()))
			{
				return new[] { milpManager.FromConstant(typedParameters.Function(x.ConstantValue.Value, y.ConstantValue.Value)) };
			}

			var one = milpManager.FromConstant(1);

			var variables =
				Enumerable.Range(0, typedParameters.ArgumentsX.Length).Select(p =>
				Enumerable.Range(0, typedParameters.ArgumentsY.Length).Select(q =>
					milpManager.CreateAnonymous(typedParameters.ArgumentMustBeOnAGrid
						? Domain.BinaryInteger
						: Domain.PositiveOrZeroReal).Set<LessOrEqual>(one)).ToArray())
				.ToArray();

			x.Set<Equal>(milpManager.Operation<Addition>(
				Enumerable.Range(0, typedParameters.ArgumentsX.Length).SelectMany(indexX =>
				Enumerable.Range(0, typedParameters.ArgumentsY.Length).Select(indexY =>
					variables[indexX][indexY].Operation<Multiplication>(milpManager.FromConstant(typedParameters.ArgumentsX.ElementAt(indexX))))).ToArray()
			));
			y.Set<Equal>(milpManager.Operation<Addition>(
				Enumerable.Range(0, typedParameters.ArgumentsX.Length).SelectMany(indexX =>
				Enumerable.Range(0, typedParameters.ArgumentsY.Length).Select(indexY =>
					variables[indexX][indexY].Operation<Multiplication>(milpManager.FromConstant(typedParameters.ArgumentsY.ElementAt(indexY))))).ToArray()
			));
			var z = milpManager.Operation<Addition>(
				Enumerable.Range(0, typedParameters.ArgumentsX.Length).SelectMany(indexX =>
				Enumerable.Range(0, typedParameters.ArgumentsY.Length).Select(indexY =>
					variables[indexX][indexY].Operation<Multiplication>(milpManager.FromConstant(typedParameters.Function(typedParameters.ArgumentsX.ElementAt(indexX), typedParameters.ArgumentsY.ElementAt(indexY)))))).ToArray()
			);

		    z.ConstantValue = x.ConstantValue.HasValue && y.ConstantValue.HasValue ? typedParameters.Function(x.ConstantValue.Value, y.ConstantValue.Value) : (double?)null;

            milpManager.Operation<Addition>(variables.SelectMany(v => v).ToArray()).Set<Equal>(one);

			var xSet = Enumerable.Range(0, typedParameters.ArgumentsX.Length).Select(indexX => milpManager.Operation<Addition>(Enumerable.Range(0, typedParameters.ArgumentsY.Length).Select(indexY => variables[indexX][indexY]).ToArray())).ToArray();
			milpManager.Set<SpecialOrderedSetType2>(xSet.First(), xSet.Skip(1).ToArray());

			var ySet = Enumerable.Range(0, typedParameters.ArgumentsY.Length).Select(indexY => milpManager.Operation<Addition>(Enumerable.Range(0, typedParameters.ArgumentsX.Length).Select(indexX => variables[indexX][indexY]).ToArray())).ToArray();
			milpManager.Set<SpecialOrderedSetType2>(ySet.First(), ySet.Skip(1).ToArray());

			if (!typedParameters.ArgumentMustBeOnAGrid)
			{
				var triangleSet = Enumerable.Range(0, typedParameters.ArgumentsY.Length).SelectMany(indexY =>
				{
					var variablesToSet = Enumerable.Range(0, typedParameters.ArgumentsX.Length).Where(indexX => indexX + indexY < variables[indexX].Length).Select(indexX => variables[indexX][indexX + indexY]).ToArray();
					if (variablesToSet.Any())
					{
						return new[] { milpManager.Operation<Addition>(variablesToSet) };
					}

					return new IVariable[0];
				}).ToArray();

				milpManager.Set<SpecialOrderedSetType2>(triangleSet.First(), triangleSet.Skip(1).ToArray());
			}
			SolverUtilities.SetExpression(z, $"approximation2D({typedParameters.FunctionDescription})");

			return new[] { z };
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			return CalculateInternal<TCompositeOperationType>(milpManager, parameters, arguments);
		}

		protected override Type[] SupportedTypes => new[] {typeof (Approximate2D)};
	}
}
