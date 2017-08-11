﻿using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class Approximate2DCalculator : ICompositeOperationCalculator
	{
		public bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters,
		   params IVariable[] arguments)
		{
			return type == CompositeOperationType.Approximate2D && arguments.Length == 2 && parameters is Approximate2DParameters;
		}

		public IEnumerable<IVariable> Calculate(IMilpManager milpManager, CompositeOperationType type, ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			if (!SupportsOperation(type, parameters, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, parameters, arguments));
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

			milpManager.Operation<Addition>(variables.SelectMany(v => v).ToArray()).Set<Equal>(one);

			var xSet = Enumerable.Range(0, typedParameters.ArgumentsX.Length).Select(indexX => milpManager.Operation<Addition>(Enumerable.Range(0, typedParameters.ArgumentsY.Length).Select(indexY => variables[indexX][indexY]).ToArray())).ToArray();
			 milpManager.Set(CompositeConstraintType.SpecialOrderedSetType2, xSet.First(), xSet.Skip(1).ToArray());

			var ySet = Enumerable.Range(0, typedParameters.ArgumentsY.Length).Select(indexY => milpManager.Operation<Addition>(Enumerable.Range(0, typedParameters.ArgumentsX.Length).Select(indexX => variables[indexX][indexY]).ToArray())).ToArray();
			milpManager.Set(CompositeConstraintType.SpecialOrderedSetType2, ySet.First(), ySet.Skip(1).ToArray());

			if (!typedParameters.ArgumentMustBeOnAGrid)
			{
				var triangleSet = Enumerable.Range(0, typedParameters.ArgumentsY.Length).SelectMany(indexY => 
				{
					var variablesToSet = Enumerable.Range(0, typedParameters.ArgumentsX.Length).Where(indexX => indexX + indexY < variables[indexX].Length).Select(indexX => variables[indexX][indexX + indexY]).ToArray();
					if (variablesToSet.Any())
					{
						return new[] {milpManager.Operation<Addition>(variablesToSet)};
					}

					return new IVariable[0];
				}).ToArray();

				milpManager.Set(CompositeConstraintType.SpecialOrderedSetType2, triangleSet.First(), triangleSet.Skip(1).ToArray());
			}
			
			z.ConstantValue = x.IsConstant() && y.IsConstant() ? typedParameters.Function(x.ConstantValue.Value, y.ConstantValue.Value) : (double?)null;
			SolverUtilities.SetExpression(z, $"approximation2D({typedParameters.FunctionDescription})");

			return new[] { z };
		}
	}
}
