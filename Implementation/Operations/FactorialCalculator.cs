using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class FactorialCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Factorial && arguments.Length == 1 &&
                   arguments.All(a => a.IsInteger() && (a.IsBinary() || a.IsPositiveOrZero()));
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                var constantArgument = (int) arguments[0].ConstantValue.Value;
                var constantResult = constantArgument == 0 ? 1 : Enumerable.Range(1, constantArgument).Aggregate((a, b) => a*b);
                return milpManager.FromConstant(constantResult);
            }
            var number = arguments[0];
            var one = milpManager.FromConstant(1);
            var result = one;
            for (int i = SoundBoundary(milpManager.MaximumIntegerValue); i >= 0; --i)
            {
                result = result.Operation(OperationType.Multiplication,
                    milpManager.Operation(OperationType.Maximum, one,
                        number.Operation(OperationType.Subtraction, milpManager.FromConstant(i))));
            }

            var finalResult = result.ChangeDomain(Domain.PositiveOrZeroInteger);
            finalResult.Expression = $"({number.Expression}!)";
            return finalResult;
        }

        private int SoundBoundary(int maximumInteger)
        {
            int size = 2;
            int factorial = 1;
            while (maximumInteger / factorial >= size)
            {
                factorial *= size;
                size ++;
            }

            return size - 1;
        }
    }
}