using System;
using System.Collections.Generic;
using MilpManager.Implementation.CompositeConstraints;
using MilpManager.Implementation.CompositeOperations;
using MilpManager.Implementation.Constraints;
using MilpManager.Implementation.Goals;
using MilpManager.Implementation.Operations;
using MilpManager.Utilities;

namespace MilpManager.Abstraction
{
	public abstract class MilpManager : IMilpManager
	{
		protected IDictionary<Type, IConstraintCalculator> Constraints = DefaultCalculators.Constraints;
		protected IDictionary<Type, ICompositeConstraintCalculator> CompositeConstraints = DefaultCalculators.CompositeConstraints;
		protected IDictionary<Type, ICompositeOperationCalculator> CompositeOperations = DefaultCalculators.CompositeOperations;
		protected IDictionary<Type, IOperationCalculator> Operations = DefaultCalculators.Operations;
		protected IDictionary<Type, IGoalCalculator> GoalCalculators = DefaultCalculators.GoalCalculators;

		public MilpManagerSettings Settings { get; }

	    protected IDictionary<string, IVariable> Variables;
	    protected int VariableIndex;
	    protected IDictionary<Tuple<object, Domain>, IVariable> CachedConstants;
        
        protected MilpManager(MilpManagerSettings settings)
		{
			Settings = settings;
		    Variables = new Dictionary<string, IVariable>();
		    CachedConstants = new Dictionary<Tuple<object, Domain>, IVariable>();
        }

		public virtual int IntegerWidth => Settings.IntegerWidth;
		public virtual int IntegerInfinity => (int) Math.Pow(2, IntegerWidth + 4) + 1;
		public virtual int MaximumIntegerValue => (int) Math.Pow(2, IntegerWidth) - 1;
		public double Epsilon => Settings.Epsilon;

		public virtual IVariable Create(string name, IVariable value)
		{
			var variable = Create(name, value.Domain.MakeNonConstant());
			variable.ConstantValue = value.ConstantValue;
			SolverUtilities.SetExpression(variable, $"{value.FullExpression()}");
			Set<Equal>(variable, value);
			return variable;
		}

		public virtual IVariable Create(IVariable value)
		{
			var variable = CreateAnonymous(value.Domain.MakeNonConstant());
			variable.ConstantValue = value.ConstantValue;
			SolverUtilities.SetExpression(variable, $"{value.FullExpression()}");
			Set<Equal>(variable, value);
			return variable;
		}

		public virtual IVariable Operation<TOperationType>(params IVariable[] variables) where TOperationType : Operation
		{
			if (Operations[typeof(TOperationType)].SupportsOperation<TOperationType>(variables))
			{
				return Operations[typeof(TOperationType)].Calculate<TOperationType>(this, variables);
			}

			throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(typeof(TOperationType), variables));
		}

		public virtual IVariable Set<TConstraintType>(IVariable left, IVariable right) where TConstraintType : Constraint
		{
			return Constraints[typeof(TConstraintType)].Set<TConstraintType>(this, left, right);
		}

		public virtual IEnumerable<IVariable> CompositeOperation<TCompositeOperationType>(params IVariable[] variables) where TCompositeOperationType : CompositeOperation
		{
			return CompositeOperation<TCompositeOperationType>(null, variables);
		}

		public virtual IEnumerable<IVariable> CompositeOperation<TCompositeOperationType>(ICompositeOperationParameters parameters, params IVariable[] variables)
			where TCompositeOperationType : CompositeOperation
		{
			if (CompositeOperations[typeof(TCompositeOperationType)].SupportsOperation<TCompositeOperationType>(parameters, variables))
			{
				return CompositeOperations[typeof(TCompositeOperationType)].Calculate<TCompositeOperationType>(this, parameters, variables);
			}

			throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(typeof(TCompositeOperationType), parameters, variables));
		}

		public virtual IVariable Set<TCompositeConstraintType>(IVariable left, params IVariable[] variables) where TCompositeConstraintType : CompositeConstraint
		{
			return Set< TCompositeConstraintType>(null, left, variables);
		}

		public virtual IVariable Set<TCompositeConstraintType>(ICompositeConstraintParameters parameters, IVariable left,
			params IVariable[] variables) where TCompositeConstraintType : CompositeConstraint
		{
			return CompositeConstraints[typeof(TCompositeConstraintType)].Set<TCompositeConstraintType>(this, parameters, left, variables);
		}

		public virtual IVariable MakeGoal<TGoalType>(params IVariable[] variables) where TGoalType : Goal
		{
			if (GoalCalculators[typeof(TGoalType)].SupportsOperation<TGoalType>(variables))
			{
				return GoalCalculators[typeof(TGoalType)].Calculate<TGoalType>(this, variables);
			}

			throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(typeof(TGoalType), variables));
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

        public virtual IVariable Create(string name, Domain domain)
        {
            var variable = InternalCreate(name, domain);
            variable.Name = name;
            variable.Domain = domain;
            variable.MilpManager = this;
            SolverUtilities.SetExpression(variable, "");
            Variables[name] = variable;
            return variable;
        }

        public virtual IVariable CreateAnonymous(Domain domain)
        {
            return Create(NewVariableName(), domain);
        }

        public virtual IVariable FromConstant(int value, Domain domain)
        {
            if (Settings.CacheConstants && CachedConstants.ContainsKey(Tuple.Create((object)value, domain)))
            {
                return CachedConstants[Tuple.Create((object)value, domain)];
            }

            var variableName = NewVariableName();
            var variable = InternalFromConstant(variableName, value, domain);
            Variables[variableName] = variable;
            variable.Name = variableName;
            variable.ConstantValue = value;
            variable.Domain = domain;
            variable.MilpManager = this;
            SolverUtilities.SetExpression(variable, $"{value}");

            if (Settings.CacheConstants)
            {
                CachedConstants[Tuple.Create((object)value, domain)] = variable;
            }

            return variable;
        }

        public virtual IVariable FromConstant(double value, Domain domain)
        {
            if (Settings.CacheConstants && CachedConstants.ContainsKey(Tuple.Create((object)value, domain)))
            {
                return CachedConstants[Tuple.Create((object)value, domain)];
            }

            var variableName = NewVariableName();
            var variable = InternalFromConstant(variableName, value, domain);
            Variables[variableName] = variable;
            variable.Name = variableName;
            variable.ConstantValue = value;
            variable.Domain = domain;
            variable.MilpManager = this;
            SolverUtilities.SetExpression(variable, $"{value}");

            if (Settings.CacheConstants)
            {
                CachedConstants[Tuple.Create((object)value, domain)] = variable;
            }

            return variable;
        }

        public virtual IVariable SumVariables(IVariable first, IVariable second, Domain domain)
        {
            var newVariable = InternalSumVariables(first, second, domain);
            newVariable.Domain = domain;
            newVariable.MilpManager = this;
            Variables[newVariable.Name] = newVariable;
            return newVariable;
        }

        public virtual IVariable NegateVariable(IVariable variable, Domain domain)
        {
            var newVariable = InternalNegateVariable(variable, domain);
            newVariable.Domain = domain;
            newVariable.MilpManager = this;
            Variables[newVariable.Name] = newVariable;
            return newVariable;
        }

        public virtual IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            var newVariable = InternalMultiplyVariableByConstant(variable, constant, domain);
            newVariable.Domain = domain;
            newVariable.MilpManager = this;
            Variables[newVariable.Name] = newVariable;
            return newVariable;
        }

        public virtual IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            var newVariable = InternalDivideVariableByConstant(variable, constant, domain);
            newVariable.Domain = domain;
            newVariable.MilpManager = this;
            Variables[newVariable.Name] = newVariable;
            return newVariable;
        }
        
		public abstract void SetLessOrEqual(IVariable variable, IVariable bound);

	    protected abstract IVariable InternalCreate(string name, Domain domain);
	    protected abstract IVariable InternalFromConstant(string name, int value, Domain domain);
	    protected abstract IVariable InternalFromConstant(string name, double value, Domain domain);
	    protected abstract IVariable InternalSumVariables(IVariable first, IVariable second, Domain domain);
	    protected abstract IVariable InternalNegateVariable(IVariable variable, Domain domain);
	    protected abstract IVariable InternalMultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain);
	    protected abstract IVariable InternalDivideVariableByConstant(IVariable variable, IVariable constant, Domain domain);

        protected virtual string NewVariableName()
	    {
	        return $"_v_{VariableIndex++}";
	    }
    }
}