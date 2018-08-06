using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;

namespace MilpManager.Implementation.Operations
{
    public class RealDivisionCalculator : BaseOperationCalculator
    {
        private static Domain CalculateDomain(IVariable[] arguments)
        {
            if (arguments.All(a => a.IsBinary()))
            {
                return Domain.BinaryInteger;
            }

            if (arguments.All(a => a.IsNonNegative()))
            {
                return Domain.PositiveOrZeroReal;
            }

            return Domain.AnyReal;
        }

        private static bool IsDividingByConstant(IVariable[] arguments)
        {
            return arguments[1].IsConstant();
        }

        private static bool AreArgumentsBinaries(IVariable[] arguments)
        {
            return arguments.All(a => a.IsBinary());
        }

        protected override bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
        {
            return arguments.Length == 2;
        }

        protected override IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
        {
            var domain = CalculateDomain(arguments);

            if (AreArgumentsBinaries(arguments))
            {
                return arguments[0];
            }

            if (IsDividingByConstant(arguments))
            {
                var finalDomain = arguments.All(x => x.IsConstant()) ? domain.MakeConstant() : domain;
                var physicalResult = milpManager.DivideVariableByConstant(arguments[0], arguments[1], finalDomain);
                physicalResult.ConstantValue = arguments[0].ConstantValue / arguments[1].ConstantValue;
                SolverUtilities.SetExpression(physicalResult, $"{arguments[0].FullExpression()} // {arguments[1].FullExpression()}");

                return physicalResult;
            }
            
            var result = milpManager.CreateAnonymous(domain);

            result.ConstantValue = arguments.All(a => a.ConstantValue.HasValue)
                ? Math.Abs(arguments[1].ConstantValue.Value) < milpManager.Epsilon
                    ? (double?)null
                    : arguments[0].ConstantValue.Value / arguments[1].ConstantValue.Value
                : null;

            var first = MultiplicationCalculator.MakePositiveIfNeeded(arguments[0]);
            var second = MultiplicationCalculator.MakePositiveIfNeeded(arguments[1]);

            result.Operation<Multiplication>(second)
                .Set<LessOrEqual>(first);

            // Constraint below is incorrect, the same issue as in Decomposition
            result.Operation<Addition>(milpManager.FromConstant(1000*milpManager.Epsilon))
                .Operation<Multiplication>(second)
                .Set<GreaterOrEqual>(first);

            result = MultiplicationCalculator.FixSign(milpManager, arguments, result);
            SolverUtilities.SetExpression(result, $"{first.FullExpression()} // {second.FullExpression()}");

            return result;
        }

        protected override IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
        {
            if (AreArgumentsBinaries(arguments))
            {
                return arguments[0];
            }

            var numerator = arguments[0].ConstantValue.Value;
            var denominator = arguments[1].ConstantValue.Value;

            if (Math.Abs(numerator % denominator) < milpManager.Epsilon)
            {
                return milpManager.FromConstant((int) (numerator / denominator));
            }

            var constantResult = numerator / denominator;
            return milpManager.FromConstant(constantResult);
        }

        protected override Type[] SupportedTypes => new[] { typeof(RealDivision) };
    }
}