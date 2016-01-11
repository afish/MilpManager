using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class ExponentationCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Exponentation && arguments.Length == 2 &&
                   ((arguments.All(a => (a.IsPositiveOrZero() || a.IsBinary()) && a.IsInteger())) || arguments.All(a => a.IsConstant()));
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                var constantResult = Math.Pow(arguments[0].ConstantValue.Value, arguments[1].ConstantValue.Value);
                if (arguments.All(a => a.IsInteger()))
                {
                    return milpManager.FromConstant((int) constantResult);
                }
                else
                {
                    return milpManager.FromConstant(constantResult);
                }
            }
            var number = arguments[0];
            var power = arguments[1];

            var one = milpManager.FromConstant(1);
            var zero = milpManager.FromConstant(0);
            var isNumberLessOrEqualOne = number.Operation(OperationType.IsLessOrEqual, one);
            var isPowerZero = power.Operation(OperationType.IsLessOrEqual, zero);
            var isPowerOne = power.Operation(OperationType.IsEqual, one);
            var isEdgeCase = milpManager.Operation(OperationType.Disjunction, isNumberLessOrEqualOne, isPowerZero, isPowerOne);
            var result = milpManager.Operation(
                OperationType.Condition,
                isPowerZero,
                one,
                milpManager.Operation(
                    OperationType.Condition,
                    isNumberLessOrEqualOne,
                    number,
                    milpManager.Operation(
                        OperationType.Condition,
                        isPowerOne,
                        number,
                        CalculatePower(number, power, milpManager, isEdgeCase)
                    )
                )
            );

            result.Expression = $"({number.Expression} ^^ {arguments[1].Expression})";
            return result;
        }

        private IVariable CalculatePower(IVariable number, IVariable power, IMilpManager milpManager, IVariable isEdgeCase)
        {
            var digits = (int)Math.Ceiling(Math.Log(milpManager.IntegerWidth, 2.0));

            var infinity = milpManager.FromConstant(milpManager.MaximumIntegerValue);
            var currentPower = milpManager.Operation(OperationType.Minimum, number, isEdgeCase.Operation(OperationType.BinaryNegation).Operation(OperationType.Multiplication, infinity));
            var decomposition = power.CompositeOperation(CompositeOperationType.UnsignedMagnitudeDecomposition).Take(digits).ToArray();
            var one = milpManager.FromConstant(1);
            var result = one;

            for (int i = 0; i < digits; ++i)
            {
                if (i > 0)
                {
                    var isAnyNonzeroDigitLater = milpManager.Operation(OperationType.Disjunction, decomposition.Skip(i).ToArray());
                    var numberToMultiply = milpManager.Operation(OperationType.Minimum, currentPower, isAnyNonzeroDigitLater.Operation(OperationType.Multiplication, infinity));
                    currentPower = numberToMultiply.Operation(OperationType.Multiplication, numberToMultiply);
                }

                result = result.Operation(OperationType.Multiplication, one.Operation(OperationType.Maximum, currentPower.Operation(OperationType.Multiplication, decomposition[i])));
            }

            return result;
        }
    }
}