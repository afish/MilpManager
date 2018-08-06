using MilpManager.Abstraction;
using MilpManager.Utilities;
using Operation = MilpManager.Abstraction.Operation;

namespace MilpManager.Implementation.CompositeOperations
{
    public abstract class GenericLexicographicalCalculator<TOperationType> : LexicographicalCompareCalculator where TOperationType : Operation
    {
        protected override IVariable CompareFinalResult(IVariable result, IMilpManager milpManager)
        {
            return result.Operation<TOperationType>(milpManager.FromConstant(0));
        }
    }
}