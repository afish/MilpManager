using System;
using MilpManager.Abstraction;

namespace MilpManager.Utilities
{
    public enum CompositeOperationType
    {
        Approximate,
        Approximate2D,
        ArrayGet,
        ArraySet,
        CountingSort,
        Decomposition,
        Loop,
        IsLexicographicalEqual,
        IsLexicographicalGreaterOrEqual,
        IsLexicographicalGreaterThan,
        IsLexicographicalLessOrEqual,
        IsLexicographicalLessThan,
        IsLexicographicalNotEqual,
        NthElements,
        OneHotEncoding,
        SelectionSort,
        UnsignedMagnitudeDecomposition
    }

    public static class CompositeOperationTypeMapper
    {
        public static Type Map(CompositeOperationType type)
        {
            switch (type)
            {
                case CompositeOperationType.Approximate:
                    return typeof(Approximate);
                case CompositeOperationType.Approximate2D:
                    return typeof(Approximate2D);
                case CompositeOperationType.ArrayGet:
                    return typeof(ArrayGet);
                case CompositeOperationType.ArraySet:
                    return typeof(ArraySet);
                case CompositeOperationType.CountingSort:
                    return typeof(CountingSort);
                case CompositeOperationType.Decomposition:
                    return typeof(Decomposition);
                case CompositeOperationType.Loop:
                    return typeof(Loop);
                case CompositeOperationType.IsLexicographicalEqual:
                    return typeof(IsLexicographicalEqual);
                case CompositeOperationType.IsLexicographicalGreaterOrEqual:
                    return typeof(IsLexicographicalGreaterOrEqual);
                case CompositeOperationType.IsLexicographicalGreaterThan:
                    return typeof(IsLexicographicalGreaterThan);
                case CompositeOperationType.IsLexicographicalLessOrEqual:
                    return typeof(IsLexicographicalLessOrEqual);
                case CompositeOperationType.IsLexicographicalLessThan:
                    return typeof(IsLexicographicalLessThan);
                case CompositeOperationType.IsLexicographicalNotEqual:
                    return typeof(IsLexicographicalNotEqual);
                case CompositeOperationType.NthElements:
                    return typeof(NthElements);
                case CompositeOperationType.OneHotEncoding:
                    return typeof(OneHotEncoding);
                case CompositeOperationType.SelectionSort:
                    return typeof(SelectionSort);
                case CompositeOperationType.UnsignedMagnitudeDecomposition:
                    return typeof(UnsignedMagnitudeDecomposition);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}