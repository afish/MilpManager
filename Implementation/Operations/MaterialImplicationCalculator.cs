using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Operations
{
	public class MaterialImplicationCalculator : BaseOperationCalculator
	{
		protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
		{
			return arguments.Length == 2 && arguments.All(a => a.IsBinary());
		}

		protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			var variable = arguments[0].Operation<BinaryNegation>().Operation<Disjunction>(arguments[1]);
			SolverUtilities.SetExpression(variable, $"{arguments[0].FullExpression()} => {arguments[1].FullExpression()}");
			return variable;
		}

		protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
		{
			return milpManager.FromConstant((int)(arguments[0].ConstantValue.Value == 0 ? 1 : arguments[1].ConstantValue.Value));
		}

		protected override Type[] SupportedTypes => new[] {typeof (MaterialImplication)};
	}
}
