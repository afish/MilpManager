using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeConstraints
{
	public class SOS2Calculator : ICompositeConstraintCalculator
	{
		public IVariable Set<TCompositeConstraintType>(IMilpManager milpManager, ICompositeConstraintParameters parameters,
			IVariable leftVariable, params IVariable[] rightVariable) where TCompositeConstraintType : CompositeConstraintType

		{
			var maximumIntegerValue = milpManager.FromConstant(milpManager.MaximumIntegerValue);
			var one = milpManager.FromConstant(1);

			var allVariables = new[] { leftVariable }.Concat(rightVariable).ToArray();
			var boundaryVariables = allVariables.Select(v => milpManager.CreateAnonymous(Domain.BinaryInteger)).ToArray();
			milpManager.Operation<Addition>(boundaryVariables).Set<LessOrEqual>(one);

			for (int i = 0; i < allVariables.Length; ++i)
			{
				IVariable sum = boundaryVariables[i];
				if (i < allVariables.Length - 1)
				{
					sum = sum.Operation<Addition>(boundaryVariables[i + 1]);
				}
				allVariables[i]
					.Set<LessOrEqual>(sum.Operation<Multiplication>(maximumIntegerValue))
					.Set<GreaterOrEqual>(sum.Operation<Multiplication>(maximumIntegerValue).Operation<Negation>());
			}

			return leftVariable;
		}
	}
}