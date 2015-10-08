using System;
using System.Linq;
using MilpManager.Abstraction;
using Domain = MilpManager.Abstraction.Domain;

namespace MilpManager.Implementation.Operations
{
    public class DivisionCalculator : IOperationCalculator
    {
        public bool SupportsOperation(OperationType type, params IVariable[] arguments)
        {
            return type == OperationType.Division && arguments.Length == 2 &&
                   (IsDividingByConstant(arguments) || arguments.All(a => (a.IsPositiveOrZero() || a.IsBinary()) && a.IsInteger()));
        }

        public IVariable Calculate(IMilpManager milpManager, OperationType type, params IVariable[] arguments)
        {
            if (!SupportsOperation(type, arguments)) throw new NotSupportedException($"Operation {type} with supplied variables [{string.Join(", ", (object[])arguments)}] not supported");
            if (arguments.All(a => a.IsConstant()))
            {
                var constantResult = arguments[0].ConstantValue.Value/arguments[1].ConstantValue.Value;
                if (arguments.All(a => a.IsInteger()))
                {
                    return milpManager.FromConstant((int) constantResult);
                }
                else
                {
                    return milpManager.FromConstant(constantResult);
                }
            }
            var domain = CalculateDomain(arguments);
            if (IsDividingByConstant(arguments))
            {
                var finalDomain = arguments.All(x => x.IsConstant()) ? domain.MakeConstant() : domain;
                var physicalResult = milpManager.DivideVariableByConstant(arguments[0], arguments[1], finalDomain);
                physicalResult.ConstantValue = arguments[0].ConstantValue/arguments[1].ConstantValue;
                physicalResult.Expression = $"({arguments[0].Expression} / {arguments[1].Expression})";
                return physicalResult;
            }

            IVariable one = milpManager.FromConstant(1);
            var result = milpManager.CreateAnonymous(domain);
            result.Operation(OperationType.Multiplication, arguments[1])
                .Set(ConstraintType.LessOrEqual, arguments[0]);
            result.Operation(OperationType.Addition, one)
                .Operation(OperationType.Multiplication, arguments[1])
                .Set(ConstraintType.GreaterOrEqual, arguments[0].Operation(OperationType.Addition, one));

            return result;
        }

        private Domain CalculateDomain(IVariable[] arguments)
        {
            if (arguments.All(a => a.IsBinary()))
            {
                return Domain.BinaryInteger;
            }

            if (IsDividingByConstant(arguments))
            {
                if (arguments.All(a => a.IsInteger()))
                {

                    if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
                    {
                        return Domain.PositiveOrZeroInteger;
                    }
                    return Domain.AnyInteger;
                }
                else
                {
                    if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
                    {
                        return Domain.PositiveOrZeroReal;
                    }
                    return Domain.AnyReal;
                }
            }

            if (arguments.All(a => a.IsPositiveOrZero() || a.IsBinary()))
            {
                return Domain.PositiveOrZeroInteger;
            }
            return Domain.AnyInteger;
        }

        private static bool IsDividingByConstant(IVariable[] arguments)
        {
            return arguments[1].IsConstant();
        }
    }
}
