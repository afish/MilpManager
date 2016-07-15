using System;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Operations
{
    public class GcdCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.GCD && arguments.Length == 2 &&
                   arguments.All(a => (a.IsPositiveOrZero() || a.IsBinary()) && a.IsInteger());
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, arguments));
            if (arguments.All(v => v.IsConstant()))
            {
                return milpManager.FromConstant(Gcd((int) arguments[0].ConstantValue.Value, (int) arguments[1].ConstantValue.Value));
            }
            var a = arguments[0];
            var b = arguments[1];
            var gcd = CalculateInternal(milpManager, a, b);

            return gcd;
        }

        private static IVariable CalculateInternal(IMilpManager milpManager, params IVariable[] arguments)
        {
            var a = arguments[0];
            var b = arguments[1];
            var gcd = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            var x = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            var y = milpManager.CreateAnonymous(Domain.PositiveOrZeroInteger);
            var m = milpManager.CreateAnonymous(Domain.AnyInteger);
            var n = milpManager.CreateAnonymous(Domain.AnyInteger);

            gcd.Set(ConstraintType.GreaterOrEqual, milpManager.FromConstant(1));
            a.Set(ConstraintType.Equal, x.Operation(OperationType.Multiplication, gcd));
            b.Set(ConstraintType.Equal, y.Operation(OperationType.Multiplication, gcd));
            gcd.Set(ConstraintType.Equal, m.Operation(OperationType.Multiplication, a).Operation(OperationType.Addition, n.Operation(OperationType.Multiplication, b)));

            gcd.ConstantValue = a.ConstantValue.HasValue && b.ConstantValue.HasValue
                ? Gcd((int) a.ConstantValue.Value, (int) b.ConstantValue.Value)
                : (double?) null;
            gcd.Expression = $"gcd({a.FullExpression()}, {b.FullExpression()})";
            return gcd;
        }
        private static int Gcd(int a, int b)
        {
            return b == 0 ? a : Gcd(b, a % b);
        }
    }
}