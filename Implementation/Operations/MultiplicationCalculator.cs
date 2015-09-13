using System;
using System.Linq;
using MilpManager.Abstraction;
using Domain = MilpManager.Abstraction.Domain;

namespace MilpManager.Implementation.Operations
{
    public class MultiplicationCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Multiplication && HasVariablesToMultiply(arguments) && (
                MultiplyBinaryVariables(arguments) || 
                MultiplyAtMostOneNonconstant(arguments) || 
                MultiplyAnyIntegers(arguments));
        }

        private static bool MultiplyAnyIntegers(IVariable[] arguments)
        {
            return arguments.All(a => a.IsInteger());
        }

        private static bool MultiplyAtMostOneNonconstant(IVariable[] arguments)
        {
            return arguments.Count(a => a.IsNotConstant()) <= 1;
        }

        private static bool MultiplyBinaryVariables(IVariable[] arguments)
        {
            return arguments.All(a => a.IsBinary());
        }

        private static bool HasVariablesToMultiply(IVariable[] arguments)
        {
            return arguments.Length >= 1;
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.Length == 1)
            {
                return arguments[0];
            }

            var domain = CalculateDomain(arguments);

            if (MultiplyAtMostOneNonconstant(arguments))
            {
                return arguments.Aggregate((x, y) =>
                {
                    var result = y.IsConstant()
                        ? milpManager.MultiplyVariableByConstant(x, y, domain)
                        : milpManager.MultiplyVariableByConstant(y, x, domain);
                    result.ConstantValue = x.ConstantValue*y.ConstantValue;
                    result.Expression = $"({x.Expression} * {y.Expression})";
                    return result;
                });
            }

            if (MultiplyBinaryVariables(arguments))
            {
                return milpManager.Operation(OperationType.Conjunction, arguments);
            }

            return MultiplyIntegers(milpManager, domain, arguments);
        }

        private IVariable MultiplyIntegers(IMilpManager baseMilpManager, Domain domain, IVariable[] arguments)
        {
            var binaries = arguments.Where(a => a.IsBinary()).ToArray();
            var nonBinaries = arguments.Where(a => !a.IsBinary()).ToArray();

            if (binaries.Any())
            {
                IVariable conjucted = baseMilpManager.Operation(OperationType.Multiplication, binaries);
                return MultipleByBinaryDigit(baseMilpManager, nonBinaries[0], conjucted).ChangeDomain(domain)
                    .Operation(OperationType.Multiplication, nonBinaries.Skip(1).ToArray());
            }

            return MultiplyNonBinaryIntegers(baseMilpManager, nonBinaries, domain);
        }

        private IVariable MultiplyNonBinaryIntegers(IMilpManager baseMilpManager, IVariable[] nonBinaries, Domain domain)
        {

            var first = nonBinaries[0];
            var second = nonBinaries[1];
            var mightBeNegatives = first.Domain == Domain.AnyInteger || first.Domain == Domain.AnyConstantInteger ||
                     second.Domain == Domain.AnyInteger || second.Domain == Domain.AnyConstantInteger;
            first = MakePositiveIfNeeded(first);
            second = MakePositiveIfNeeded(second);

            var zero = baseMilpManager.FromConstant(0);
            var result = MakeLongMultiplication(baseMilpManager, domain, zero, second, first);
            result = FixSign(baseMilpManager, nonBinaries, mightBeNegatives, zero, result);
            result = result.ChangeDomain(domain);

            return result.Operation(OperationType.Multiplication, nonBinaries.Skip(2).ToArray());
        }

        private IVariable FixSign(IMilpManager baseMilpManager, IVariable[] nonBinaries, bool mightBeNegatives, IVariable zero,
            IVariable result)
        {
            if (mightBeNegatives)
            {
                var sign =
                    nonBinaries[0].Operation(OperationType.IsGreaterOrEqual, zero)
                        .Operation(OperationType.IsEqual,
                            nonBinaries[1].Operation(OperationType.IsGreaterOrEqual, zero));
                var two = baseMilpManager.FromConstant(2);
                result =
                    MultipleByBinaryDigit(baseMilpManager, result, sign)
                        .Operation(OperationType.Subtraction,
                            result.Operation(OperationType.Division, two))
                        .Operation(OperationType.Multiplication, two);
            }
            return result;
        }

        private IVariable MakeLongMultiplication(IMilpManager baseMilpManager, Domain domain, IVariable zero, IVariable second,
            IVariable first)
        {
            var result = zero;

            var secondDigits = second.CompositeOperation(CompositeOperationType.UnsignedMagnitudeDecomposition).ToArray();
            for (int index = 0, power = 1; index < secondDigits.Length; ++index, power = power*2)
            {
                result = result.Operation(OperationType.Addition,
                    MultipleByBinaryDigit(baseMilpManager, first, secondDigits[index])
                        .ChangeDomain(domain)
                        .Operation(OperationType.Multiplication, baseMilpManager.FromConstant(power))
                    );
            }
            return result;
        }

        private IVariable MakePositiveIfNeeded(IVariable variable)
        {
            if (variable.Domain == Domain.AnyInteger || variable.Domain == Domain.AnyConstantInteger)
            {
                variable = variable.Operation(OperationType.AbsoluteValue);
            }

            return variable;
        }

        private IVariable MultipleByBinaryDigit(IMilpManager baseMilpManager, IVariable number, IVariable digit)
        {
            if (number.Domain == Domain.AnyConstantInteger || number.Domain == Domain.AnyInteger)
            {
                var absoluteNumber = number.Operation(OperationType.AbsoluteValue);
                var result = MultipleByBinaryDigit(baseMilpManager, absoluteNumber, digit);
                var two = baseMilpManager.FromConstant(2);
                return MultipleByBinaryDigit(baseMilpManager, result, number.Operation(OperationType.IsGreaterOrEqual, baseMilpManager.FromConstant(0)))
                        .Operation(OperationType.Subtraction,
                            result.Operation(OperationType.Division, two))
                        .Operation(OperationType.Multiplication, two);
            }

            IVariable digitMultipliedByInfinity = digit.Operation(OperationType.Multiplication, baseMilpManager.FromConstant(baseMilpManager.MaximumIntegerValue));
            return baseMilpManager.Operation(OperationType.Minimum,
                number,
                digitMultipliedByInfinity
                );
        }

        private Domain CalculateDomain(IVariable[] arguments)
        {
            Domain domain;
            if (MultiplyBinaryVariables(arguments))
            {
                domain = Domain.BinaryInteger;
            }
            else if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
            {
                domain = arguments.Any(a => a.IsReal()) ? Domain.PositiveOrZeroReal : Domain.PositiveOrZeroInteger;
            }
            else
            {
                domain = arguments.Any(a => a.IsReal()) ? Domain.AnyReal : Domain.AnyInteger;
            }

            return arguments.All(a => a.IsConstant()) ? domain.MakeConstant() : domain;
        }
    }
}
