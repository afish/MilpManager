using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class IsLexicographicalGreaterThanCalculator : GenericLexicographicalCalculator<IsGreaterThan>
	{
		public IsLexicographicalGreaterThanCalculator() : base(CompositeOperationType.IsLexicographicalGreaterThan) { }
		protected override int ConstantFinalResult(int result)
		{
			return result;
		}

		protected override string ComparerFinalResult => "?>";
	}
}