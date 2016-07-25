using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation
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
    }
}