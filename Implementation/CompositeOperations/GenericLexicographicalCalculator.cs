using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.CompositeOperations
{
    public abstract class GenericLexicographicalCalculator<TOperationType> : LexicographicalCompareCalculator where TOperationType : OperationType
    {
        protected override IVariable CompareFinalResult(IVariable result, IMilpManager milpManager)
        {
            return result.Operation<TOperationType>(milpManager.FromConstant(0));
        }
    }
}