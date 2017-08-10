using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public class IsLexicographicalEqualCalculator : GenericLexicographicalCalculator<IsEqual>
	{
		public IsLexicographicalEqualCalculator() : base(CompositeOperationType.IsLexicographicalEqual) { }
		protected override int ConstantFinalResult(int result)
		{
			return result == 0 ? 1 : 0;
		}

		protected override string ComparerFinalResult => "?==";
	}
}