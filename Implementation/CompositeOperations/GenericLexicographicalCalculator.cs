using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public abstract class GenericLexicographicalCalculator : LexicographicalCompareCalculator
    {
        private readonly CompositeOperationType _compositeType;
        private readonly OperationType _operationType;

        protected GenericLexicographicalCalculator(CompositeOperationType compositeType, OperationType operationType)
        {
            _compositeType = compositeType;
            _operationType = operationType;
        }

        public override bool SupportsOperation(CompositeOperationType type, ICompositeOperationParameters parameters,
            params IVariable[] arguments)
        {
            return base.SupportsOperation(type, parameters, arguments) && type == _compositeType;
        }

        protected override IVariable CompareFinalResult(IVariable result, IMilpManager milpManager)
        {
            return result.Operation(_operationType, milpManager.FromConstant(0));
        }
    }

    public class IsLexicographicalGreaterThanCalculator : GenericLexicographicalCalculator
    {
        public IsLexicographicalGreaterThanCalculator() : base(CompositeOperationType.IsLexicographicalGreaterThan, OperationType.IsGreaterThan) { }
        protected override int ConstantFinalResult(int result)
        {
            return result;
        }

        protected override string ComparerFinalResult => "?>";
    }

    public class IsLexicographicalLessThanCalculator : GenericLexicographicalCalculator
    {
        public IsLexicographicalLessThanCalculator() : base(CompositeOperationType.IsLexicographicalLessThan, OperationType.IsLessThan) { }
        protected override int ConstantFinalResult(int result)
        {
            return result < 0 ? 1 : 0;
        }

        protected override string ComparerFinalResult => "?<";
    }

    public class IsLexicographicalGreaterOrEqualCalculator : GenericLexicographicalCalculator
    {
        public IsLexicographicalGreaterOrEqualCalculator() : base(CompositeOperationType.IsLexicographicalGreaterOrEqual, OperationType.IsGreaterOrEqual) { }
        protected override int ConstantFinalResult(int result)
        {
            return result >= 0 ? 1 : 0;
        }

        protected override string ComparerFinalResult => "?>=";
    }

    public class IsLexicographicalLessOrEqualCalculator : GenericLexicographicalCalculator
    {
        public IsLexicographicalLessOrEqualCalculator() : base(CompositeOperationType.IsLexicographicalLessOrEqual, OperationType.IsLessOrEqual) { }
        protected override int ConstantFinalResult(int result)
        {
            return result <= 0 ? 1 : 0;
        }

        protected override string ComparerFinalResult => "?<=";
    }

    public class IsLexicographicalEqualCalculator : GenericLexicographicalCalculator
    {
        public IsLexicographicalEqualCalculator() : base(CompositeOperationType.IsLexicographicalEqual, OperationType.IsEqual) { }
        protected override int ConstantFinalResult(int result)
        {
            return result == 0 ? 1 : 0;
        }

        protected override string ComparerFinalResult => "?==";
    }

    public class IsLexicographicalNotEqualCalculator : GenericLexicographicalCalculator
    {
        public IsLexicographicalNotEqualCalculator() : base(CompositeOperationType.IsLexicographicalNotEqual, OperationType.IsNotEqual) { }
        protected override int ConstantFinalResult(int result)
        {
            return result != 0 ? 1 : 0;
        }

        protected override string ComparerFinalResult => "?!=";
    }
}