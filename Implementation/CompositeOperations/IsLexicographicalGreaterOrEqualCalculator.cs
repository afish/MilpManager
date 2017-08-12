using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class IsLexicographicalGreaterOrEqualCalculator : GenericLexicographicalCalculator<IsGreaterOrEqual>
	{
		protected override int ConstantFinalResult(int result)
		{
			return result >= 0 ? 1 : 0;
		}

		protected override string ComparerFinalResult => "?>=";
		protected override Type[] SupportedTypes => new[] { typeof(IsLexicographicalGreaterOrEqual) };
	}
}