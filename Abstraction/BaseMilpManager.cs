using System;
using System.Collections.Generic;
using MilpManager.Implementation;
using MilpManager.Implementation.CompositeConstraints;
using MilpManager.Implementation.CompositeOperations;
using MilpManager.Implementation.Constraints;
using MilpManager.Implementation.Goals;
using MilpManager.Implementation.Operations;

namespace MilpManager.Abstraction
{
    public abstract class BaseMilpManager : IMilpManager
    {
        protected IDictionary<Type, IConstraintCalculator> Constraints => new Dictionary
            <Type, IConstraintCalculator>
        {
            {typeof(Equal), new CanonicalConstraintCalculator()},
            {typeof(GreaterOrEqual), new CanonicalConstraintCalculator()},
            {typeof(GreaterThan), new CanonicalConstraintCalculator()},
            {typeof(LessOrEqual), new CanonicalConstraintCalculator()},
            {typeof(LessThan), new CanonicalConstraintCalculator()},
            {typeof(NotEqual), new CanonicalConstraintCalculator()},
            {typeof(MultipleOf), new MultipleOfCalculator()}
        };

        protected IDictionary<CompositeConstraintType, ICompositeConstraintCalculator> CompositeConstraints => new Dictionary
            <CompositeConstraintType, ICompositeConstraintCalculator>
        {
            {CompositeConstraintType.AllDifferent, new AllDifferentCalculator()},
            {CompositeConstraintType.Cardinality, new CardinalityCalculator()},
            {CompositeConstraintType.FromSet, new FromSetCalculator()},
            {CompositeConstraintType.NotFromSet, new NotFromSetCalculator()},
            {CompositeConstraintType.SpecialOrderedSetType1, new SOS1Calculator()},
            {CompositeConstraintType.SpecialOrderedSetType2, new SOS2Calculator()}
        };

        protected IDictionary<CompositeOperationType, ICompositeOperationCalculator> CompositeOperations => new Dictionary
            <CompositeOperationType, ICompositeOperationCalculator>
        {
            {CompositeOperationType.Approximate, new ApproximateCalculator()},
            {CompositeOperationType.Approximate2D, new Approximate2DCalculator()},
            {CompositeOperationType.ArrayGet, new ArrayGetCalculator()},
            {CompositeOperationType.ArraySet, new ArraySetCalculator()},
            {CompositeOperationType.CountingSort, new CountingSortCalculator()},
            {CompositeOperationType.Decomposition, new DecompositionCalculator()},
            {CompositeOperationType.IsLexicographicalEqual, new IsLexicographicalEqualCalculator()},
            {CompositeOperationType.IsLexicographicalGreaterOrEqual, new IsLexicographicalGreaterOrEqualCalculator()},
            {CompositeOperationType.IsLexicographicalGreaterThan, new IsLexicographicalGreaterThanCalculator()},
            {CompositeOperationType.IsLexicographicalLessOrEqual, new IsLexicographicalLessOrEqualCalculator()},
            {CompositeOperationType.IsLexicographicalLessThan, new IsLexicographicalLessThanCalculator()},
            {CompositeOperationType.IsLexicographicalNotEqual, new IsLexicographicalNotEqualCalculator()},
            {CompositeOperationType.Loop, new LoopCalculator()},
            {CompositeOperationType.NthElements, new NthElementsCalculator()},
            {CompositeOperationType.SelectionSort, new SelectionSortCalculator()},
            {CompositeOperationType.UnsignedMagnitudeDecomposition, new UnsignedMagnitudeDecompositionCalculator()}
        };

        protected IDictionary<Type, IOperationCalculator> Operations => new Dictionary
            <Type, IOperationCalculator>
        {
            {typeof(AbsoluteValue), new AbsoluteValueCalculator()},
            {typeof(Addition), new AdditionCalculator()},
            {typeof(BinaryNegation), new BinaryNegationCalculator()},
            {typeof(Condition), new ConditionCalculator()},
            {typeof(Conjunction), new ConjunctionCalculator()},
            {typeof(DifferentValuesCount), new DifferentValuesCountCalculator()},
            {typeof(Disjunction), new DisjunctionCalculator()},
            {typeof(Division), new DivisionCalculator()},
            {typeof(Equivalency), new EquivalencyCalculator()},
            {typeof(ExclusiveDisjunction), new ExclusiveDisjunctionCalculator()},
            {typeof(Exponentiation), new ExponentiationCalculator()},
            {typeof(Factorial), new FactorialCalculator()},
            {typeof(GCD), new GcdCalculator()},
            {typeof(Subtraction), new SubtractionCalculator()},
            {typeof(MaterialImplication), new MaterialImplicationCalculator()},
            {typeof(Multiplication), new MultiplicationCalculator()},
            {typeof(IsEqual), new IsEqualCalculator()},
            {typeof(IsGreaterOrEqual), new IsGreaterOrEqualCalculator()},
            {typeof(IsGreaterThan), new IsGreaterThanCalculator()},
            {typeof(IsLessOrEqual), new IsLessOrEqualCalculator()},
            {typeof(IsLessThan), new IsLessThanCalculator()},
            {typeof(IsNotEqual), new IsNotEqualCalculator()},
            {typeof(Maximum), new MaximumMinimumCalculator()},
            {typeof(Minimum), new MaximumMinimumCalculator()},
            {typeof(Negation), new NegationCalculator()},
            {typeof(Remainder), new RemainderCalculator()}
        };

        protected IDictionary<GoalType, IGoalCalculator>  GoalCalculators => new Dictionary<GoalType, IGoalCalculator>
        {
            {GoalType.Minimize, new MinimizeCalculator() },
            {GoalType.Maximize, new MaximizeCalculator() },
            {GoalType.MaximizeMinium, new MaximizeMinimumCalculator() },
            {GoalType.MinimizeMaximum, new MinimizeMaximumCalculator() },
            {GoalType.MaximizeMaximum, new MaximizeMaximumCalculator() },
            {GoalType.MinimizeMinimum, new MinimizeMinimumCalculator() }
        };

        protected BaseMilpManager(int integerWidth, double epsilon)
        {
            IntegerWidth = integerWidth;
            Epsilon = epsilon;
        }

        public virtual int IntegerWidth { get; private set; }

        public virtual int IntegerInfinity => (int) Math.Pow(2, IntegerWidth + 4) + 1;

        public virtual int MaximumIntegerValue => (int) Math.Pow(2, IntegerWidth) - 1;

        public double Epsilon { get; }

        public virtual IVariable Create(string name, IVariable value)
        {
            var variable = Create(name, value.Domain.MakeNonConstant());
            variable.ConstantValue = value.ConstantValue;
            variable.Expression = $"{value.FullExpression()}";
            Set<Equal>(variable, value);
            return variable;
        }

        public virtual IVariable Create(IVariable value)
        {
            var variable = CreateAnonymous(value.Domain.MakeNonConstant());
            variable.ConstantValue = value.ConstantValue;
            variable.Expression = $"{value.FullExpression()}";
            Set<Equal>(variable, value);
            return variable;
        }

        public virtual IVariable Operation<TOperationType>(params IVariable[] variables) where TOperationType : OperationType
        {
            if (Operations[typeof(TOperationType)].SupportsOperation<TOperationType>(variables))
            {
                return Operations[typeof(TOperationType)].Calculate<TOperationType>(this, variables);
            }

            throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(typeof(TOperationType), variables));
        }

        public virtual IVariable Set<TConstraintType>(IVariable left, IVariable right) where TConstraintType : ConstraintType
        {
            return Constraints[typeof(TConstraintType)].Set<TConstraintType>(this, left, right);
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

            throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, parameters, variables));
        }

        public virtual IVariable Set(CompositeConstraintType type, IVariable left, params IVariable[] variables)
        {
            return Set(type, null, left, variables);
        }

        public virtual IVariable Set(CompositeConstraintType type, ICompositeConstraintParameters parameters, IVariable left,
            params IVariable[] variables)
        {
            return CompositeConstraints[type].Set(this, type, parameters, left, variables);
        }

        public IVariable MakeGoal(GoalType type, params IVariable[] variables)
        {
            if (GoalCalculators[type].SupportsOperation(type, variables))
            {
                return GoalCalculators[type].Calculate(this, type, variables);
            }

            throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(type, variables));
        }

        public virtual void SetGreaterOrEqual(IVariable variable, IVariable bound)
        {
            bound.Set<LessOrEqual>(variable);
        }

        public virtual void SetEqual(IVariable variable, IVariable bound)
        {
            bound.Set<LessOrEqual>(variable);
            variable.Set<LessOrEqual>(bound);
        }

        public virtual IVariable FromConstant(double value)
        {
            return FromConstant(value, value < 0 ? Domain.AnyConstantReal : Domain.PositiveOrZeroConstantReal);
        }

        public virtual IVariable FromConstant(int value)
        {
            var domain = value < 0
                ? Domain.AnyConstantInteger
                : value > 1 ? Domain.PositiveOrZeroConstantInteger : Domain.BinaryConstantInteger;
            return FromConstant(value, domain);
        }

        public abstract IVariable SumVariables(IVariable first, IVariable second, Domain domain);
        public abstract IVariable NegateVariable(IVariable variable, Domain domain);
        public abstract IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain);
        public abstract IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain);
        public abstract void SetLessOrEqual(IVariable variable, IVariable bound);
        public abstract IVariable FromConstant(int value, Domain domain);
        public abstract IVariable FromConstant(double value, Domain domain);
        public abstract IVariable Create(string name, Domain domain);
        public abstract IVariable CreateAnonymous(Domain domain);
    }
}