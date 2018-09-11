using System;
using MilpManager.Abstraction;

namespace MilpManager.Utilities
{
    public enum CompositeConstraintType
    {
        AllDifferent,
        Cardinality,
        Composite,
        FromSet,
        NotFromSet,
        SpecialOrderedSetType1,
        SpecialOrderedSetType2
    }

    public static class CompositeConstraintTypeMapper
    {
        public static Type Map(CompositeConstraintType type)
        {
            switch (type)
            {
                case CompositeConstraintType.AllDifferent:
                    return typeof(AllDifferent);
                case CompositeConstraintType.Cardinality:
                    return typeof(Cardinality);
                case CompositeConstraintType.Composite:
                    return typeof(Composite);
                case CompositeConstraintType.FromSet:
                    return typeof(FromSet);
                case CompositeConstraintType.NotFromSet:
                    return typeof(NotFromSet);
                case CompositeConstraintType.SpecialOrderedSetType1:
                    return typeof(SpecialOrderedSetType1);
                case CompositeConstraintType.SpecialOrderedSetType2:
                    return typeof(SpecialOrderedSetType2);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}