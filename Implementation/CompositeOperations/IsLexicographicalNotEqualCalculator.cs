using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class IsLexicographicalNotEqualCalculator : GenericLexicographicalCalculator<IsNotEqual>
	{
		public IsLexicographicalNotEqualCalculator() : base(CompositeOperationType.IsLexicographicalNotEqual) { }
		protected override int ConstantFinalResult(int result)
		{
			return result != 0 ? 1 : 0;
		}

		protected override string ComparerFinalResult => "?!=";
	}
}