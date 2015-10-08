using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class RemainderCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Remainder && arguments.Length == 2 && arguments.All(x => x.IsPositiveOrZero() || x.IsBinary()) && arguments.All(a => a.IsInteger());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                var constantResult = (int) arguments[0].ConstantValue.Value%(int) arguments[1].ConstantValue.Value;
                return milpManager.FromConstant((int)constantResult);
            }
            IVariable numerator = arguments[0];
            IVariable denominator = arguments[1];

            var one = milpManager.FromConstant(1);
            var any = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            any.Operation(OperationType.Multiplication, denominator).Set(ConstraintType.LessOrEqual, numerator);
            any.Operation(OperationType.Addition, one)
                .Operation(OperationType.Multiplication, denominator)
                .Set(ConstraintType.GreaterOrEqual, numerator.Operation(OperationType.Addition, one));
            
            IVariable result = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            result.Set(ConstraintType.LessOrEqual, denominator);
            result.Set(ConstraintType.Equal,
                numerator.Operation(OperationType.Subtraction,
                    denominator.Operation(OperationType.Multiplication, any)));

            return result;
        }
    }
}