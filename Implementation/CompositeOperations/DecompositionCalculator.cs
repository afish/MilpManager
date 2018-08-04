using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class DecompositionCalculator : BaseCompositeOperationCalculator
	{
		private IEnumerable<IVariable> CalculateForVariable(IMilpManager milpManager, IVariable[] arguments,
		    uint decompositionBase, ICompositeOperationParameters parameters)
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

		    double?[] constants = Enumerable.Range(0, 150).Select(x => (double?)null).ToArray(); 

            if (arguments.All(a => a.ConstantValue.HasValue))
		    {
		        constants = Weights(milpManager, parameters, arguments).Select(w => (double?)w).ToArray();
		    }

		    var digitsCount = GetDigitsCount(milpManager, decompositionBase);
		    List<Tuple<IVariable, int>> variables =
				Enumerable.Range(0, digitsCount)
					.Select(i =>
				    {
				        var variable = GetVariable();
                        variable.ConstantValue = constants[i];
				        return Tuple.Create(variable, (int)Math.Pow(decompositionBase, i));
				    })
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
		            .Select((p, i) =>
		            {
		                var variable = GetVariable();
		                variable.ConstantValue = constants[digitsCount + i];
		                return Tuple.Create(variable, p);
		            })
		            .ToList();

		        sum = sum.Operation<Addition>(fraction
		            .Select(v => v.Item1.Operation<Multiplication>(milpManager.FromConstant(v.Item2))).ToArray());

		        sum.Set<LessOrEqual>(arguments[0]);
                
                // Constraint below is incorrect!
                // Mathematically it should be GreaterThan and we should add fraction.Last().Item2 but then we would hit representation issues
                // Let's decompose 1.0/3 in base 2 with precision 0.001
                // We should get 0.010101010 which is 0.33203125. But when we add 1 in the last place we get 0.333984375
                // Let's compare numbers, we have 0.333984375 - 0.333333333 = 0.000651042 < 0.001 so we cannot compare those numbers with given precision
                // But when we have GreaterOrEqual, we have two representations for numbers like 0.5 in base 2. One is 0.10..., the other one is 0.01111...
                // We sacrifice precision to avoid breaking the model
                sum.Operation<Addition>(milpManager.FromConstant(fraction.Last().Item2))
                    .Set<GreaterOrEqual>(arguments[0]);

		        resultVariables.AddRange(fraction.Select(v => v.Item1));
            }

		    return resultVariables.Select((v, index) => {
				var result = v;
				SolverUtilities.SetExpression(result, $"decomposition(digit: {index}, base: {decompositionBase}, {arguments[0].FullExpression()})");
				return result;
			});
		}

		public static int GetDigitsCount(IMilpManager milpManager, uint decompositionBase)
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
			foreach (var i in CalculateForVariable(milpManager, arguments, decompositionBase, parameters))
			{
				yield return i;
			}
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
		    return Weights(milpManager, parameters, arguments).Select(milpManager.FromConstant);
		}

	    private IEnumerable<int> Weights(IMilpManager milpManager,
	        ICompositeOperationParameters parameters, params IVariable[] arguments)
	    {
	        var decompositionBase = ((DecompositionParameters)parameters).Base;

	        uint currentValue = (uint)arguments[0].ConstantValue.Value;
	        for (int i = 0; i < GetDigitsCount(milpManager, decompositionBase); ++i)
	        {
	            yield return (int)(currentValue % decompositionBase);
	            currentValue /= decompositionBase;
	        }

	        if (arguments[0].IsReal())
	        {
	            double fraction = arguments[0].ConstantValue.Value - (uint)arguments[0].ConstantValue.Value;
	            double precision = 1.0 / decompositionBase;
	            while (precision >= milpManager.Epsilon)
	            {
	                var quantity = (int)(fraction / precision);
	                yield return quantity;
	                fraction -= quantity * precision;

	                precision /= decompositionBase;
	            }
	        }
        }


        protected override Type[] SupportedTypes => new[] {typeof (Decomposition)};
	}
}