using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeOperations
{
	public class OneHotEncodingCalculator : BaseCompositeOperationCalculator
	{
		protected override bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments)
		{
			return parameters is OneHotEncodingParameters && ((OneHotEncodingParameters)parameters).MaximumValue > 0 && arguments.Length == 1 && arguments[0].IsInteger() && arguments[0].IsNonNegative();
		}

		protected override IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
			var typedParameters = parameters as OneHotEncodingParameters;

			var variables = new List<IVariable>();

		    for (int i = 0; i <= typedParameters.MaximumValue; ++i)
		    {
		        var singleVariable = arguments[0].Operation<IsEqual>(milpManager.FromConstant(i));
		        SolverUtilities.SetExpression(singleVariable, $"oneHotEncoding(index: i, {arguments[0].FullExpression()})");

		        variables.Add(singleVariable);
            }

			return variables;
		}

		protected override IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager,
			ICompositeOperationParameters parameters, params IVariable[] arguments)
		{
		    var typedParameters = parameters as OneHotEncodingParameters;

            return Enumerable.Range(0, (int)(typedParameters.MaximumValue + 1)).Select(i => milpManager.FromConstant(Math.Abs(arguments[0].ConstantValue.Value - i) < milpManager.Epsilon ? 1 : 0));
        }

		protected override Type[] SupportedTypes => new[] {typeof (OneHotEncoding)};
	}
}