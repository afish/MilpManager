using System;
using MilpManager.Abstraction;

namespace MilpManager.Utilities
{
    public enum OperationType
    {
        AbsoluteValue,
        Addition,
        BinaryNegation,
        Condition,
        Conjunction,
        DifferentValuesCount,
        Disjunction,
        Division,
        Equivalency,
        ExclusiveDisjunction,
        Exponentiation,
        GCD,
        Factorial,
        IsEqual,
        IsGreaterOrEqual,
        IsGreaterThan,
        IsLessOrEqual,
        IsLessThan,
        IsNotEqual,
        MaterialImplication,
        Maximum,
        Minimum,
        Multiplication,
        Negation,
        Subtraction,
        Remainder
    }

    public static class OperationTypeMapper
    {
        public static Type Map(OperationType type)
        {
            switch (type)
            {
                case OperationType.AbsoluteValue:
                    return typeof(AbsoluteValue);
                case OperationType.Addition:
                    return typeof(Addition);
                case OperationType.BinaryNegation:
                    return typeof(BinaryNegation);
                case OperationType.Condition:
                    return typeof(Condition);
                case OperationType.Conjunction:
                    return typeof(Conjunction);
                case OperationType.DifferentValuesCount:
                    return typeof(DifferentValuesCount);
                case OperationType.Disjunction:
                    return typeof(Disjunction);
                case OperationType.Division:
                    return typeof(Division);
                case OperationType.Equivalency:
                    return typeof(Equivalency);
                case OperationType.ExclusiveDisjunction:
                    return typeof(ExclusiveDisjunction);
                case OperationType.Exponentiation:
                    return typeof(Exponentiation);
                case OperationType.GCD:
                    return typeof(GCD);
                case OperationType.Factorial:
                    return typeof(Factorial);
                case OperationType.IsEqual:
                    return typeof(IsEqual);
                case OperationType.IsGreaterOrEqual:
                    return typeof(IsGreaterOrEqual);
                case OperationType.IsGreaterThan:
                    return typeof(IsGreaterThan);
                case OperationType.IsLessOrEqual:
                    return typeof(IsLessOrEqual);
                case OperationType.IsLessThan:
                    return typeof(IsLessThan);
                case OperationType.IsNotEqual:
                    return typeof(IsNotEqual);
                case OperationType.MaterialImplication:
                    return typeof(MaterialImplication);
                case OperationType.Maximum:
                    return typeof(Maximum);
                case OperationType.Minimum:
                    return typeof(Minimum);
                case OperationType.Multiplication:
                    return typeof(Multiplication);
                case OperationType.Negation:
                    return typeof(Negation);
                case OperationType.Subtraction:
                    return typeof(Subtraction);
                case OperationType.Remainder:
                    return typeof(Remainder);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}