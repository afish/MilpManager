using System;
using MilpManager.Abstraction;

namespace MilpManager.Utilities
{
    public enum ConstraintType
    {
        Equal,
        GreaterOrEqual,
        GreaterThan,
        LessOrEqual,
        LessThan,
        MultipleOf,
        NotEqual
    }

    public static class ConstraintTypeMapper
    {
        public static Type Map(ConstraintType type)
        {
            switch (type)
            {
                case ConstraintType.Equal:
                    return typeof(Equal);
                case ConstraintType.GreaterOrEqual:
                    return typeof(GreaterOrEqual);
                case ConstraintType.GreaterThan:
                    return typeof(GreaterThan);
                case ConstraintType.LessOrEqual:
                    return typeof(LessOrEqual);
                case ConstraintType.LessThan:
                    return typeof(LessThan);
                case ConstraintType.MultipleOf:
                    return typeof(MultipleOf);
                case ConstraintType.NotEqual:
                    return typeof(NotEqual);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}