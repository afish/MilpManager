using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class IsLexicographicalGreaterThanCalculator : GenericLexicographicalCalculator<IsGreaterThan>
	{
		protected override int ConstantFinalResult(int result)
		{
			return result;
		}

		protected override string ComparerFinalResult => "?>";
		protected override Type[] SupportedTypes => new[] { typeof(IsLexicographicalGreaterThan) };
	}
}