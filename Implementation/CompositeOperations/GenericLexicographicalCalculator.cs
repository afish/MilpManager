using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public abstract class GenericLexicographicalCalculator<TOperationType> : LexicographicalCompareCalculator where TOperationType : OperationType
    {
        private readonly CompositeOperationType _compositeType;

        protected GenericLexicographicalCalculator(CompositeOperationType compositeType)
        {
            _compositeType = compositeType;
        }

        public override bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            return base.SupportsOperation(type, parameters, arguments) && type == _compositeType;
        }

        protected override IVariable CompareFinalResult(IVariable result, IMilpManager milpManager)
        {
            return result.Operation<TOperationType>(milpManager.FromConstant(0));
        }
    }
}