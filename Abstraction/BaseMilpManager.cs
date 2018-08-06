using System;
using System.Collections.Generic;
using MilpManager.Implementation;
using MilpManager.Implementation.CompositeConstraints;
using MilpManager.Implementation.CompositeOperations;
using MilpManager.Implementation.Constraints;
using MilpManager.Implementation.Goals;
using MilpManager.Implementation.Operations;
using MilpManager.Utilities;

namespace MilpManager.Abstraction
{
	public abstract class BaseMilpManager : IMilpManager
	{
		protected IDictionary<Type, IConstraintCalculator> Constraints = DefaultCalculators.Constraints;

		protected IDictionary<Type, ICompositeConstraintCalculator> CompositeConstraints = DefaultCalculators.CompositeConstraints;

		protected IDictionary<Type, ICompositeOperationCalculator> CompositeOperations = DefaultCalculators.CompositeOperations;

		protected IDictionary<Type, IOperationCalculator> Operations = DefaultCalculators.Operations;

		protected IDictionary<Type, IGoalCalculator> GoalCalculators = DefaultCalculators.GoalCalculators;

		public MilpManagerSettings Settings { get; }

		protected BaseMilpManager(MilpManagerSettings settings)
		{
			Settings = settings;
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

		public virtual IEnumerable<IVariable> CompositeOperation<TCompositeOperationType>(params IVariable[] variables) where TCompositeOperationType : CompositeOperationType
		{
			return CompositeOperation<TCompositeOperationType>(null, variables);
		}

		public virtual IEnumerable<IVariable> CompositeOperation<TCompositeOperationType>(ICompositeOperationParameters parameters, params IVariable[] variables)
			where TCompositeOperationType : CompositeOperationType
		{
			if (CompositeOperations[typeof(TCompositeOperationType)].SupportsOperation<TCompositeOperationType>(parameters, variables))
			{
				return CompositeOperations[typeof(TCompositeOperationType)].Calculate<TCompositeOperationType>(this, parameters, variables);
			}

			throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(typeof(TCompositeOperationType), parameters, variables));
		}

		public virtual IVariable Set<TCompositeConstraintType>(IVariable left, params IVariable[] variables) where TCompositeConstraintType : CompositeConstraintType
		{
			return Set< TCompositeConstraintType>(null, left, variables);
		}

		public virtual IVariable Set<TCompositeConstraintType>(ICompositeConstraintParameters parameters, IVariable left,
			params IVariable[] variables) where TCompositeConstraintType : CompositeConstraintType
		{
			return CompositeConstraints[typeof(TCompositeConstraintType)].Set<TCompositeConstraintType>(this, parameters, left, variables);
		}

		public IVariable MakeGoal<TGoalType>(params IVariable[] variables) where TGoalType : GoalType
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