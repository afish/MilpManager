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
                    return typeof(Approximate);
                case CompositeOperationType.ArrayGet:
                    return typeof(Approximate);
                case CompositeOperationType.ArraySet:
                    return typeof(Approximate);
                case CompositeOperationType.CountingSort:
                    return typeof(Approximate);
                case CompositeOperationType.Decomposition:
                    return typeof(Approximate);
                case CompositeOperationType.Loop:
                    return typeof(Approximate);
                case CompositeOperationType.IsLexicographicalEqual:
                    return typeof(Approximate);
                case CompositeOperationType.IsLexicographicalGreaterOrEqual:
                    return typeof(Approximate);
                case CompositeOperationType.IsLexicographicalGreaterThan:
                    return typeof(Approximate);
                case CompositeOperationType.IsLexicographicalLessOrEqual:
                    return typeof(Approximate);
                case CompositeOperationType.IsLexicographicalLessThan:
                    return typeof(Approximate);
                case CompositeOperationType.IsLexicographicalNotEqual:
                    return typeof(Approximate);
                case CompositeOperationType.NthElements:
                    return typeof(Approximate);
                case CompositeOperationType.SelectionSort:
                    return typeof(Approximate);
                case CompositeOperationType.UnsignedMagnitudeDecomposition:
                    return typeof(Approximate);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}