using System;
using System.Linq;
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
            var number = arguments[0];
            var one = milpManager.FromConstant(1);
            var result = one;
            for (int i = SoundBoundary(milpManager.MaximumIntegerValue); i >= 0; --i)
            {
                result = result.Operation(OperationType.Multiplication,
                    milpManager.Operation(OperationType.Maximum, one,
                        number.Operation(OperationType.Subtraction, milpManager.FromConstant(i))));
            }

            return result;
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