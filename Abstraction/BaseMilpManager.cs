using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Implementation;
using MilpManager.Implementation.Comparisons;
using MilpManager.Implementation.CompositeOperations;
using MilpManager.Implementation.Operations;

namespace MilpManager.Abstraction
{
    public abstract class BaseMilpManager : IMilpManager
    {
        protected readonly IDictionary<ConstraintType, IConstraintCalculator> Comparisons = new Dictionary
            <ConstraintType, IConstraintCalculator>
        {
            {ConstraintType.Equal, new CanonicalConstraintCalculator()},
            {ConstraintType.LessOrEqual, new CanonicalConstraintCalculator()},
            {ConstraintType.GreaterOrEqual, new CanonicalConstraintCalculator()},
            {ConstraintType.MultipleOf, new MultipleOfCalculator()}
        };

        protected readonly IDictionary<CompositeOperationType, ICompositeOperationCalculator> CompositeOperations = new Dictionary
            <CompositeOperationType, ICompositeOperationCalculator>
        {
            {CompositeOperationType.UnsignedMagnitudeDecomposition, new UnsignedMagnitudeDecompositionCalculator()},
            {CompositeOperationType.Sort, new SortCalculator()},
            {CompositeOperationType.NthElements, new NthElementsCalculator()}
        };

        protected readonly IDictionary<OperationType, IOperationCalculator> Operations = new Dictionary
            <OperationType, IOperationCalculator>
        {
            {OperationType.Addition, new AdditionCalculator()},
            {OperationType.Subtraction, new SubtractionCalculator()},
            {OperationType.Multiplication, new MultiplicationCalculator()},
            {OperationType.Division, new DivisionCalculator()},
            {OperationType.Conjunction, new ConjunctionCalculator()},
            {OperationType.Disjunction, new DisjunctionCalculator()},
            {OperationType.IsGreaterThan, new IsGreaterThanCalculator()},
            {OperationType.IsLessThan, new IsLessThanCalculator()},
            {OperationType.BinaryNegation, new BinaryNegationCalculator()},
            {OperationType.IsEqual, new IsEqualCalculator()},
            {OperationType.IsNotEqual, new IsNotEqualCalculator()},
            {OperationType.Negation, new NegationCalculator()},
            {OperationType.AbsoluteValue, new AbsoluteValueCalculator()},
            {OperationType.Maximum, new MaximumMinimumCalculator()},
            {OperationType.Minimum, new MaximumMinimumCalculator()},
            {OperationType.Condition, new ConditionCalculator()},
            {OperationType.IsGreaterOrEqual, new IsGreaterOrEqualCalculator()},
            {OperationType.IsLessOrEqual, new IsLessOrEqualCalculator()},
            {OperationType.Remainder, new RemainderCalculator()}
        };

        public BaseMilpManager(int integerWidth)
        {
            IntegerWidth = integerWidth;
        }

        public BaseMilpManager()
        {
            IntegerWidth = 10;
        }

        public int IntegerWidth { get; private set; }

        public int IntegerInfinity
        {
            get { return (int) Math.Pow(2, IntegerWidth + 4) + 1; }
        }

        public int MaximumIntegerValue
        {
            get { return (int) Math.Pow(2, IntegerWidth) - 1; }
        }

        public virtual IVariable Create(string name, IVariable value)
        {
            var variable = Create(name,
                SelectDomainForConstant(value));
            Set(ConstraintType.Equal, variable, value);
            return variable;
        }

        private static Domain SelectDomainForConstant(IVariable value)
        {
            return value.Domain == Domain.AnyConstantInteger
                ? Domain.AnyInteger
                : value.Domain == Domain.AnyConstantReal
                    ? Domain.AnyReal
                    : value.Domain == Domain.BinaryConstantInteger
                        ? Domain.BinaryInteger
                        : value.Domain == Domain.PositiveOrZeroConstantInteger
                            ? Domain.PositiveOrZeroInteger
                            : value.Domain == Domain.PositiveOrZeroConstantReal
                                ? Domain.PositiveOrZeroReal
                                : value.Domain;
        }

        public virtual IVariable Create(IVariable value)
        {
            var variable = CreateAnonymous(SelectDomainForConstant(value));
            Set(ConstraintType.Equal, variable, value);
            return variable;
        }

        public virtual IVariable Operation(OperationType type, params IVariable[] variables)
        {
            if (Operations[type].SupportsOperation(type, variables))
            {
                return Operations[type].Calculate(this, type, variables);
            }

            throw new NotSupportedException("Operation " + type + " with supplied variables [" +
                                            string.Join(", ", variables.Select(v => v.Domain.ToString()).ToArray()) +
                                            "] not supported");
        }

        public virtual IVariable Set(ConstraintType type, IVariable left, IVariable right)
        {
            return Comparisons[type].Set(this, type, left, right);
        }

        public virtual IEnumerable<IVariable> CompositeOperation(CompositeOperationType type,
            params IVariable[] variables)
        {
            return CompositeOperation(type, null, variables);
        }

        public virtual IEnumerable<IVariable> CompositeOperation(CompositeOperationType type,
            ICompositeOperationParameters parameters, params IVariable[] variables)
        {
            if (CompositeOperations[type].SupportsOperation(type, parameters, variables))
            {
                return CompositeOperations[type].Calculate(this, type, parameters, variables);
            }

            throw new NotSupportedException("Operation " + type + " with supplied variables not supported");
        }

        public virtual void SetGreaterOrEqual(IVariable variable, IVariable bound)
        {
            bound.Set(ConstraintType.LessOrEqual, variable);
        }

        public virtual void SetEqual(IVariable variable, IVariable bound)
        {
            bound.Set(ConstraintType.LessOrEqual, variable);
            variable.Set(ConstraintType.LessOrEqual, bound);
        }

        public abstract IVariable SumVariables(IVariable first, IVariable second, Domain domain);
        public abstract IVariable NegateVariable(IVariable variable, Domain domain);
        public abstract IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain);
        public abstract IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain);
        public abstract void SetLessOrEqual(IVariable variable, IVariable bound);

        public virtual IVariable FromConstant(int value)
        {
            var domain = value < 0
                ? Domain.AnyConstantInteger
                : value > 1 ? Domain.PositiveOrZeroConstantInteger : Domain.BinaryConstantInteger;
            return FromConstant(value, domain);
        }

        public abstract IVariable FromConstant(int value, Domain domain);
        public abstract IVariable FromConstant(double value, Domain domain);

        public virtual IVariable FromConstant(double value)
        {
            return FromConstant(value, value < 0 ? Domain.AnyConstantReal : Domain.PositiveOrZeroConstantReal);
        }

        public abstract IVariable Create(string name, Domain domain);
        public abstract IVariable CreateAnonymous(Domain domain);
    }
}