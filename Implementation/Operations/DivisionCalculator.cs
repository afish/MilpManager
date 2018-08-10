using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;
using Domain = MilpManager.Abstraction.Domain;

namespace MilpManager.Implementation.Operations
{
	public class DivisionCalculator : BaseOperationCalculator
	{
		private static Domain CalculateDomain(IVariable[] arguments)
		{
			if (arguments.All(a => a.IsBinary()))
			{
				return Domain.BinaryInteger;
			}

			if (arguments.All(a => a.IsNonNegative()))
			{
			    return Domain.PositiveOrZeroInteger;
			}

		    return Domain.AnyInteger;
		}

	    private static bool AreArgumentsBinaries(IVariable[] arguments)
	    {
	        return arguments.All(a => a.IsBinary());
	    }

        protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2;
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var domain = CalculateDomain(arguments);

		    if (AreArgumentsBinaries(arguments))
		    {
		        return arguments[0];
		    }
            
			var result = milpManager.CreateAnonymous(domain);

		    result.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
		        ? arguments[1].ConstantValue.Value == 0
		            ? (double?)null
		            : (long)(arguments[0].ConstantValue.Value / arguments[1].ConstantValue.Value)
		        : null;

		    var first = MultiplicationCalculator.MakePositiveIfNeeded(arguments[0]);
		    var second = MultiplicationCalculator.MakePositiveIfNeeded(arguments[1]);

            result.Operation<Multiplication>(second)
				.Set<LessOrEqual>(first);

		    result.Operation<Addition>(milpManager.FromConstant(1))
		        .Operation<Multiplication>(second)
                .Set<GreaterThan>(first);

            result = MultiplicationCalculator.FixSign(milpManager, arguments, result).ChangeDomain(domain);
            SolverUtilities.SetExpression(result, $"{first.FullExpression()} / {second.FullExpression()}");

            return result;
        }

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var constantResult = arguments[0].ConstantValue.Value / arguments[1].ConstantValue.Value;
			return milpManager.FromConstant((int)constantResult);
		}

		protected override Type[] SupportedTypes => new[] {typeof (Division)};
	}
}
