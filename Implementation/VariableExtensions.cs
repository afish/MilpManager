using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation
{
    public static class VariableExtensions
    {
        public static IVariable Create(this IVariable variable, string name)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.MilpManager.Create(name, variable);
        }

        public static IVariable Create(this IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.MilpManager.Create(variable);
        }

        public static IVariable Operation(this IVariable variable, OperationType type, params IVariable[] variables)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.MilpManager.Operation(type, new[]{variable}.Concat(variables).ToArray());
        }

        public static IEnumerable<IVariable> CompositeOperation(this IVariable variable, CompositeOperationType type, params IVariable[] variables)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.MilpManager.CompositeOperation(type, new[]{variable}.Concat(variables).ToArray());
        }

        public static IVariable Set(this IVariable variable, ConstraintType type, IVariable right)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.MilpManager.Set(type, variable, right);
        }

        public static IVariable Set(this IVariable variable, CompositeConstraintType type, ICompositeConstraintParameters parameters, params IVariable[] right)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.MilpManager.Set(type, parameters, variable, right);
        }

        public static IVariable MakeGoal(this IVariable variable, GoalType type, params IVariable[] variables)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.MilpManager.MakeGoal(type, new[] { variable }.Concat(variables).ToArray());
        }

        public static double GetValue(this IVariable variable)
        {
            if (!(variable.MilpManager is IMilpSolver))
            {
                throw new InvalidOperationException("MilpManager doesn't allow to obtain values");
            }
            return ((IMilpSolver) variable.MilpManager).GetValue(variable);
        }

        public static IVariable ChangeDomain(this IVariable variable, Domain newDomain)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            var newVariable = variable.MilpManager.CreateAnonymous(newDomain);
            variable.Set(ConstraintType.Equal, newVariable);

            return newVariable;
        }

        public static bool IsReal(this IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.Domain == Domain.AnyConstantReal || variable.Domain == Domain.AnyReal ||
               variable.Domain == Domain.PositiveOrZeroConstantReal || variable.Domain == Domain.PositiveOrZeroReal;
        }

        public static bool IsInteger(this IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return !variable.IsReal();
        }

        public static bool IsConstant(this IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.Domain == Domain.AnyConstantInteger || variable.Domain == Domain.AnyConstantReal ||
               variable.Domain == Domain.BinaryConstantInteger || variable.Domain == Domain.PositiveOrZeroConstantInteger ||
               variable.Domain == Domain.PositiveOrZeroConstantReal;
        }

        public static bool IsNotConstant(this IVariable variable)
        {
            return !variable.IsConstant();
        }

        public static bool IsBinary(this IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.Domain == Domain.BinaryConstantInteger || variable.Domain == Domain.BinaryInteger;
        }

        public static bool IsPositiveOrZero(this IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return variable.Domain == Domain.PositiveOrZeroConstantInteger || variable.Domain == Domain.PositiveOrZeroConstantReal ||
               variable.Domain == Domain.PositiveOrZeroInteger || variable.Domain == Domain.PositiveOrZeroReal;
        }

        public static string FullExpression(this IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            if (variable.ConstantValue.HasValue)
            {
                if (variable.IsConstant())
                {
                    return $"{variable.Name}[{variable.ConstantValue}]{{={variable.Expression}}}";
                }
                else
                {
                    return $"{variable.Name}[{variable.ConstantValue}?{variable.Domain}]{{={variable.Expression}}}";
                }
            }
            else
            {
                return $"{variable.Name}[{variable.Domain}]{{={variable.Expression}}}";
            }
        }

        public static VariableWrapper Wrap(this IVariable variable)
        {
            return new VariableWrapper(variable);
        }

        public static IVariable MakeTrue(this IVariable variable)
        {
            return variable.Set(ConstraintType.Equal, variable.MilpManager.FromConstant(1));
        }

        public static IVariable MakeFalse(this IVariable variable)
        {
            return variable.Set(ConstraintType.Equal, variable.MilpManager.FromConstant(0));
        }
    }
}
