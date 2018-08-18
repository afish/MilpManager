using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Operations
{
	public class GreatestPowerDividingCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2 && arguments.All(a => a.IsInteger() && a.IsNonNegative()) && arguments[1].IsConstant() && arguments[1].ConstantValue.Value >= 1.0;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
		    if (Math.Abs(arguments[1].ConstantValue.Value - 1.0) < milpManager.Epsilon)
		    {
		        return milpManager.FromConstant(1);
		    }

            var power = (int)arguments[1].ConstantValue.Value;
		    var powers = Enumerable.Range(0, 32).Select(p => (int)Math.Pow(power, p)).Where(p => p >= 0 && p <= milpManager.MaximumIntegerValue).ToArray();
		    var variables = powers.Select(milpManager.FromConstant).Select(p => arguments[0].Operation<Remainder>(p).Operation<IsEqual>(milpManager.FromConstant(0)).Operation<Multiplication>(p)).ToArray();
		    var result = milpManager.Operation<Maximum>(variables.ToArray());

		    result.ConstantValue = arguments[0].ConstantValue.HasValue ? (double?)powers.LastOrDefault(p => p <= arguments[0].ConstantValue.Value) : null;
		    SolverUtilities.SetExpression(result, $"greatestPowerDividing{arguments[0].FullExpression()} power = {arguments[1].FullExpression()}");
		    return result;
        }

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
		    if (Math.Abs(arguments[1].ConstantValue.Value - 1.0) < milpManager.Epsilon)
		    {
		        return milpManager.FromConstant(1);
		    }

            int result = 1;
		    int power = (int)arguments[1].ConstantValue.Value;

		    var target = (int)arguments[0].ConstantValue.Value;
		    while (milpManager.MaximumIntegerValue / power >= result && target % (result * power) == 0)
		    {
		        result *= power;
		    }

		    return milpManager.FromConstant(result);
        }

		protected override Type[] SupportedTypes => new[] {typeof (GreatestPowerDividing)};
	}
}
