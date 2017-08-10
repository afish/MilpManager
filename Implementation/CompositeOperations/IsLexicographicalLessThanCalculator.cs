using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class IsLexicographicalLessThanCalculator : GenericLexicographicalCalculator<IsLessThan>
	{
		public IsLexicographicalLessThanCalculator() : base(CompositeOperationType.IsLexicographicalLessThan) { }
		protected override int ConstantFinalResult(int result)
		{
			return result < 0 ? 1 : 0;
		}

		protected override string ComparerFinalResult => "?<";
	}
}