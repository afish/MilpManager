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

        public IVariable Calculate(BaseMilpManager baseMilpManager, OperationType type, params IVariable[] arguments)
        {
            if (arguments.Length == 1)
            {
                return arguments[0];
            }

            var domain = CalculateDomain(arguments);

            if (MultiplyAtMostOneNonconstant(arguments))
            {
                return arguments.Aggregate((x,y) => baseMilpManager.MultiplyVariableByConstant(x, y, domain));
            }

            if (MultiplyBinaryVariables(arguments))
            {
                return baseMilpManager.Operation(OperationType.Conjunction, arguments);
            }

            return MultiplyIntegers(baseMilpManager, domain, arguments);
        }

        private IVariable MultiplyIntegers(BaseMilpManager baseMilpManager, Domain domain, IVariable[] arguments)
        {
            var binaries = arguments.Where(a => a.IsBinary()).ToArray();
            var nonBinaries = arguments.Where(a => !a.IsBinary()).ToArray();

            if (binaries.Any())
            {
                IVariable conjucted = binaries.Length > 1 ? baseMilpManager.Operation(OperationType.Conjunction, binaries) : binaries[0];
                return MultipleByBinaryDigit(baseMilpManager, nonBinaries[0], conjucted)
                    .Operation(OperationType.Multiplication, nonBinaries.Skip(1).ToArray());
            }

            var first = nonBinaries[0];
            var second = nonBinaries[1];
            var mightBeNegatives = first.Domain == Domain.AnyInteger || first.Domain == Domain.AnyConstantInteger ||
                     second.Domain == Domain.AnyInteger || second.Domain == Domain.AnyConstantInteger;
            first = MakePositiveIfNeeded(first);
            second = MakePositiveIfNeeded(second);
            
            var secondDigits = second.CompositeOperation(CompositeOperationType.UnsignedMagnitudeDecomposition).ToArray();
            var zero = baseMilpManager.FromConstant(0);
            var result = zero;

            for (int index = 0, power = 1; index < secondDigits.Length; ++index, power = power * 2)
            {
                result = result.Operation(OperationType.Addition,
                    MultipleByBinaryDigit(baseMilpManager, first, secondDigits[index]).ChangeDomain(domain).Operation(OperationType.Multiplication, baseMilpManager.FromConstant(power))
                );
            }

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

            result = result.ChangeDomain(domain);

            return result.Operation(OperationType.Multiplication, nonBinaries.Skip(2).ToArray());
        }

        private IVariable MakePositiveIfNeeded(IVariable variable)
        {
            if (variable.Domain == Domain.AnyInteger || variable.Domain == Domain.AnyConstantInteger)
            {
                variable = variable.Operation(OperationType.AbsoluteValue);
            }

            return variable;
        }

        private IVariable MultipleByBinaryDigit(BaseMilpManager baseMilpManager, IVariable number, IVariable digit)
        {
            if (number.Domain == Domain.AnyConstantInteger || number.Domain == Domain.AnyInteger)
            {
                var absoluteNumber = number.Operation(OperationType.AbsoluteValue);
                var result = MultipleByBinaryDigit(baseMilpManager, absoluteNumber, digit).ChangeDomain(Domain.PositiveOrZeroInteger);
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
            if (MultiplyBinaryVariables(arguments))
            {
                return arguments.Any(a => a.IsNotConstant()) ? Domain.BinaryInteger : Domain.BinaryConstantInteger;
            }

            if (arguments.All(a => a.IsPositiveOrZero()))
            {
                if (arguments.Any(a => a.IsReal()))
                {
                    return arguments.Any(a => a.IsNotConstant()) ? Domain.PositiveOrZeroReal : Domain.PositiveOrZeroConstantReal;
                }

                return arguments.Any(a => a.IsNotConstant()) ? Domain.PositiveOrZeroInteger : Domain.PositiveOrZeroConstantInteger;
            }

            if (arguments.Any(a => a.IsReal()))
            {
                return arguments.Any(a => a.IsNotConstant()) ? Domain.AnyReal : Domain.AnyConstantReal;
            }

            return arguments.Any(a => a.IsNotConstant()) ? Domain.AnyInteger : Domain.AnyConstantInteger;
        }
    }
}
