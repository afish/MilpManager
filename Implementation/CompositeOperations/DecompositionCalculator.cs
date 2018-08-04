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
		    IVariable GetVariable()
		    {
		        var variable = milpManager.CreateAnonymous(decompositionBase == 2
		            ? Domain.BinaryInteger
		            : Domain.PositiveOrZeroInteger);
		        if (decompositionBase > 2)
		        {
		            variable = variable.Set<LessOrEqual>(milpManager.FromConstant((int) decompositionBase - 1));
		        }

		        return variable;
		    }

		    List<Tuple<IVariable, int>> variables =
				Enumerable.Range(0, GetDigitsCount(milpManager, decompositionBase))
					.Select(i => Tuple.Create(GetVariable(), (int)Math.Pow(decompositionBase, i)))
					.ToList();

		    var sum = milpManager.Operation<Addition>(
		        variables.Select(v => v.Item1.Operation<Multiplication>(milpManager.FromConstant(v.Item2)))
		            .ToArray()
		        );

		    List<IVariable> resultVariables = variables.Select(v => v.Item1).ToList();
            
            if (arguments[0].IsInteger())
		    {
		        sum.Set<Equal>(arguments[0]);
		    }
		    else
		    {
		        List<Tuple<IVariable, double>> fraction = Enumerable.Range(1, 100)
		            .Select(i => Math.Pow(decompositionBase, -i)).Where(p => p >= milpManager.Epsilon)
		            .Select(p => Tuple.Create(GetVariable(), p))
		            .ToList();

		        sum = sum.Operation<Addition>(fraction
		            .Select(v => v.Item1.Operation<Multiplication>(milpManager.FromConstant(v.Item2))).ToArray());

		        sum.Set<LessOrEqual>(arguments[0]);
		        sum.Operation<Addition>(milpManager.FromConstant(1)
		                .Operation<Multiplication>(milpManager.FromConstant(fraction.Last().Item2)))
		            .Set<GreaterThan>(arguments[0]);

		        resultVariables.AddRange(fraction.Select(v => v.Item1));
            }

		    return resultVariables.Select((v, index) => {
				var result = v;
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
				   ((DecompositionParameters)parameters).Base >= 2 && arguments.Length == 1 && arguments[0].IsNonNegative();
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
            
		    uint currentValue = (uint) arguments[0].ConstantValue.Value;
		    for (int i = 0; i < GetDigitsCount(milpManager, decompositionBase); ++i)
		    {
		        yield return milpManager.FromConstant((int) (currentValue % decompositionBase));
		        currentValue /= decompositionBase;
		    }

		    if (arguments[0].IsReal())
		    {
		        double fraction = arguments[0].ConstantValue.Value - (uint)arguments[0].ConstantValue.Value;
		        double precision = 1.0 / decompositionBase;
		        while (precision >= milpManager.Epsilon)
		        {
		            var quantity = (int)(fraction / precision);
		            yield return milpManager.FromConstant(quantity);
		            fraction -= quantity * precision;

		            precision /= decompositionBase;
		        }
		    }
		}

		protected override Type[] SupportedTypes => new[] {typeof (Decomposition)};
	}
}