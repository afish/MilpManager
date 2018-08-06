using System;
using MilpManager.Abstraction;

namespace MilpManager.Utilities
{
    public static class DomainExtensions
    {
        /// <summary>
        /// Returns constant counterpart of a domain
        /// </summary>
        /// <param name="domain"></param>
        /// <returns>Constant counterpart of a domain</returns>
        public static Domain MakeConstant(this Domain domain)
        {
            switch (domain)
            {
                case Domain.AnyInteger:
                    return Domain.AnyConstantInteger;
                case Domain.AnyReal:
                    return Domain.AnyConstantReal;
                case Domain.PositiveOrZeroInteger:
                    return Domain.PositiveOrZeroConstantInteger;
                case Domain.PositiveOrZeroReal:
                    return Domain.PositiveOrZeroConstantReal;
                case Domain.BinaryInteger:
                    return Domain.BinaryConstantInteger;
                case Domain.AnyConstantInteger:
                case Domain.AnyConstantReal:
                case Domain.PositiveOrZeroConstantInteger:
                case Domain.PositiveOrZeroConstantReal:
                case Domain.BinaryConstantInteger:
                    return domain;
                default:
                    throw new ArgumentOutOfRangeException(nameof(domain), domain, null);
            }
        }

        /// <summary>
        /// Returns non-constant counterpart of a domain
        /// </summary>
        /// <param name="domain"></param>
        /// <returns>Non-constant counterpart of a domain</returns>
        public static Domain MakeNonConstant(this Domain domain)
        {
            switch (domain)
            {
                case Domain.AnyInteger:
                case Domain.AnyReal:
                case Domain.PositiveOrZeroInteger:
                case Domain.PositiveOrZeroReal:
                case Domain.BinaryInteger:
                    return domain;
                case Domain.AnyConstantInteger:
                    return Domain.AnyInteger;
                case Domain.AnyConstantReal:
                    return Domain.AnyReal;
                case Domain.PositiveOrZeroConstantInteger:
                    return Domain.PositiveOrZeroInteger;
                case Domain.PositiveOrZeroConstantReal:
                    return Domain.PositiveOrZeroReal;
                case Domain.BinaryConstantInteger:
                    return Domain.BinaryInteger;
                default:
                    throw new ArgumentOutOfRangeException(nameof(domain), domain, null);
            }
        }

        /// <summary>
        /// Indicates whether domain represents real number
        /// </summary>
        /// <param name="domain">Domain to examine</param>
        /// <returns>True when domain represents real number</returns>
        public static bool IsReal(this Domain domain)
        {
            return domain == Domain.AnyConstantReal || domain == Domain.AnyReal ||
               domain == Domain.PositiveOrZeroConstantReal || domain == Domain.PositiveOrZeroReal;
        }

        /// <summary>
        /// Indicates whether domain represents integer value
        /// </summary>
        /// <param name="domain">Domain to examine</param>
        /// <returns>True when domain represents integer value</returns>
        public static bool IsInteger(this Domain domain)
        {
            return !domain.IsReal();
        }

        /// <summary>
        /// Indicates whether domain represents constant value
        /// </summary>
        /// <param name="domain">Domain to examine</param>
        /// <returns>True when domain represents constant value</returns>
        public static bool IsConstant(this Domain domain)
        {
            return domain == Domain.AnyConstantInteger || domain == Domain.AnyConstantReal ||
               domain == Domain.BinaryConstantInteger || domain == Domain.PositiveOrZeroConstantInteger ||
               domain == Domain.PositiveOrZeroConstantReal;
        }

        /// <summary>
        /// Indicates whether domain represents non-constant value
        /// </summary>
        /// <param name="domain">Domain to examine</param>
        /// <returns>True when domain represents non-constant value</returns>
        public static bool IsNotConstant(this Domain domain)
        {
            return !domain.IsConstant();
        }

        /// <summary>
        /// Indicates whether domain represents binary value
        /// </summary>
        /// <param name="domain">Domain to examine</param>
        /// <returns>True when domain represent's binary value</returns>
        public static bool IsBinary(this Domain domain)
        {
            return domain == Domain.BinaryConstantInteger || domain == Domain.BinaryInteger;
        }

        /// <summary>
        /// Indicates whether domain represents positive or zero value (and not binary one)
        /// </summary>
        /// <param name="domain">Domain to examine</param>
        /// <returns>True when domain represents positive or zero value (and not binary one)</returns>
        public static bool IsPositiveOrZero(this Domain domain)
        {
            return domain == Domain.PositiveOrZeroConstantInteger || domain == Domain.PositiveOrZeroConstantReal ||
               domain == Domain.PositiveOrZeroInteger || domain == Domain.PositiveOrZeroReal;
        }

        /// <summary>
        /// Indicates whether domain represents non-negative value (binary || positive or zero)
        /// </summary>
        /// <param name="domain">Domain to examine</param>
        /// <returns>True when domain represents non-negative value (binary || positive or zero)</returns>
        public static bool IsNonNegative(this Domain domain)
        {
            return domain.IsPositiveOrZero() || domain.IsBinary();
        }

        /// <summary>
        /// Calculate lowest domain capable of holding values of both domains
        /// </summary>
        /// <param name="first">First domain to consider</param>
        /// <param name="second">Second domain to consider</param>
        /// <returns>Lowest domain capable of holding both variables</returns>
        public static Domain LowestEncompassingDomain(this Domain first, Domain second)
        {
            if (first.IsBinary())
            {
                return second.MakeNonConstant();
            }
            if (second.IsBinary())
            {
                return first.MakeNonConstant();
            }

            if (first.IsInteger() && second.IsInteger())
            {
                if (first.IsPositiveOrZero() && second.IsPositiveOrZero())
                {
                    return Domain.PositiveOrZeroInteger;
                }
                else
                {
                    return Domain.AnyInteger;
                }
            }

            if (first.IsPositiveOrZero() && second.IsPositiveOrZero())
            {
                return Domain.PositiveOrZeroReal;
            }

            return Domain.AnyReal;
        }
    }
}