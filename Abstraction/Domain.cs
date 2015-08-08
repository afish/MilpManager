using System;

namespace MilpManager.Abstraction
{
    [Serializable]
    public enum Domain
    {
        AnyInteger,
        AnyReal,
        PositiveOrZeroInteger,
        PositiveOrZeroReal,
        BinaryInteger,
        AnyConstantInteger,
        AnyConstantReal,
        PositiveOrZeroConstantInteger,
        PositiveOrZeroConstantReal,
        BinaryConstantInteger
    }
}