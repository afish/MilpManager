using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class IsLexicographicalLessOrEqualCalculator : GenericLexicographicalCalculator<IsLessOrEqual>
	{
		public IsLexicographicalLessOrEqualCalculator() : base(CompositeOperationType.IsLexicographicalLessOrEqual) { }
		protected override int ConstantFinalResult(int result)
		{
			return result <= 0 ? 1 : 0;
		}

		protected override string ComparerFinalResult => "?<=";
	}
}