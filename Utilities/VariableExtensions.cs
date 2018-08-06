using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Utilities
{
	public static class VariableExtensions
	{
		/// <summary>
		/// Materializes variable in a solver with given name
		/// </summary>
		/// <param name="variable">Variable to materialize</param>
		/// <param name="name">Name of materialized variable</param>
		/// <returns>Variable representing materialized variable</returns>
		public static IVariable Create(this IVariable variable, string name)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.MilpManager.Create(name, variable);
		}

		/// <summary>
		/// Materializes variable in a solver
		/// </summary>
		/// <param name="variable">Variable to materialize</param>
		/// <returns>Variable representing materialized variable</returns>
		public static IVariable Create(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.MilpManager.Create(variable);
		}

		/// <summary>
		/// Performs operation
		/// </summary>
		/// <typeparam name="TOperationType">Operation type</typeparam>
		/// <param name="variable">Variable to perform operation on</param>
		/// <param name="variables">Operation arguments</param>
		/// <returns>Operation result</returns>
		public static IVariable Operation<TOperationType>(this IVariable variable, params IVariable[] variables) where TOperationType : Operation
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.MilpManager.Operation<TOperationType>(new[]{variable}.Concat(variables).ToArray());
		}

		/// <summary>
		/// Performs operation
		/// </summary>
		/// <param name="variable">Variable to perform operation on</param>
		/// <param name="operation">Operation type</param>
		/// <param name="variables">Operation arguments</param>
		/// <returns>Operation result</returns>
		public static IVariable Operation(this IVariable variable, Type operation, params IVariable[] variables)
		{
			return (IVariable) typeof (VariableExtensions)
				.GetMethod(nameof(Operation), new[] {typeof (IVariable), typeof (IVariable[])})
				.MakeGenericMethod(operation)
				.Invoke(null, new object[] {variable, variables});
	    }

	    /// <summary>
	    /// Performs operation
	    /// </summary>
	    /// <param name="variable">Variable to perform operation on</param>
	    /// <param name="operationType">Operation type</param>
	    /// <param name="variables">Operation arguments</param>
	    /// <returns>Operation result</returns>
	    public static IVariable Operation(this IVariable variable, OperationType operationType, params IVariable[] variables)
	    {
	        return Operation(variable, OperationTypeMapper.Map(operationType), variables);
	    }

        /// <summary>
        /// Performs composite operation
        /// </summary>
        /// <typeparam name="TCompositeOperationType">Operation type</typeparam>
        /// <param name="variable">Variable to perform operation on</param>
        /// <param name="variables">Operation arguments</param>
        /// <returns>Operation result</returns>
        public static IEnumerable<IVariable> CompositeOperation<TCompositeOperationType>(this IVariable variable, params IVariable[] variables) where TCompositeOperationType : CompositeOperation
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.MilpManager.CompositeOperation<TCompositeOperationType>(new[]{variable}.Concat(variables).ToArray());
	    }

        /// <summary>
        /// Performs composite operation
        /// </summary>
        /// <param name="variable">Variable to perform operation on</param>
        /// <param name="compositeOperation">Operation type</param>
        /// <param name="variables">Operation arguments</param>
        /// <returns>Operation result</returns>
        public static IEnumerable<IVariable> CompositeOperation(this IVariable variable, Type compositeOperation, params IVariable[] variables)
	    {
	        return (IEnumerable<IVariable>)typeof(VariableExtensions)
	            .GetMethod(nameof(CompositeOperation), new[] { typeof(IVariable), typeof(IVariable[]) })
	            .MakeGenericMethod(compositeOperation)
	            .Invoke(null, new object[] { variable, variables });
	    }

	    /// <summary>
	    /// Performs composite operation
	    /// </summary>
	    /// <param name="variable">Variable to perform operation on</param>
	    /// <param name="compositeOperation">Operation type</param>
	    /// <param name="variables">Operation arguments</param>
	    /// <returns>Operation result</returns>
	    public static IEnumerable<IVariable> CompositeOperation(this IVariable variable, CompositeOperationType compositeOperation, params IVariable[] variables)
	    {
	        if (variable == null) throw new ArgumentNullException(nameof(variable));
            return CompositeOperation(variable, CompositeOperationTypeMapper.Map(compositeOperation), variables);
	    }

        /// <summary>
        /// Performs composite operation
        /// </summary>
        /// <typeparam name="TCompositeOperationType">Operation type</typeparam>
        /// <param name="variable">Variable to perform operation on</param>
        /// <param name="parameters">Operation parameters</param>
        /// <param name="variables">Operation arguments</param>
        /// <returns>Operation result</returns>
        public static IEnumerable<IVariable> CompositeOperation<TCompositeOperationType>(this IVariable variable, ICompositeOperationParameters parameters, params IVariable[] variables) where TCompositeOperationType : CompositeOperation
	    {
	        if (variable == null) throw new ArgumentNullException(nameof(variable));
	        return variable.MilpManager.CompositeOperation<TCompositeOperationType>(parameters, new[] { variable }.Concat(variables).ToArray());
	    }

        /// <summary>
        /// Performs composite operation
        /// </summary>
        /// <param name="variable">Variable to perform operation on</param>
        /// <param name="compositeOperation">Operation type</param>
        /// <param name="parameters">Operation parameters</param>
        /// <param name="variables">Operation arguments</param>
        /// <returns>Operation result</returns>
        public static IEnumerable<IVariable> CompositeOperation(this IVariable variable, Type compositeOperation, ICompositeOperationParameters parameters, params IVariable[] variables)
	    {
	        return (IEnumerable<IVariable>)typeof(VariableExtensions)
	            .GetMethod(nameof(CompositeOperation), new[] { typeof(IVariable), typeof(ICompositeOperationParameters), typeof(IVariable[]) })
	            .MakeGenericMethod(compositeOperation)
	            .Invoke(null, new object[] { variable, parameters, variables });
        }

	    /// <summary>
	    /// Performs composite operation
	    /// </summary>
	    /// <param name="variable">Variable to perform operation on</param>
	    /// <param name="compositeOperation">Operation type</param>
	    /// <param name="parameters">Parameters of operation</param>
	    /// <param name="variables">Operation arguments</param>
	    /// <returns>Operation result</returns>
	    public static IEnumerable<IVariable> CompositeOperation(this IVariable variable, CompositeOperationType compositeOperation, ICompositeOperationParameters parameters, params IVariable[] variables)
	    {
	        if (variable == null) throw new ArgumentNullException(nameof(variable));
            return CompositeOperation(variable, CompositeOperationTypeMapper.Map(compositeOperation), parameters, variables);
	    }

        /// <summary>
        /// Adds constraint to a solver
        /// </summary>
        /// <typeparam name="TConstraintType">Constraint type</typeparam>
        /// <param name="variable">Variable to constrain</param>
        /// <param name="right">Right hand side of a constraint</param>
        /// <returns>Variable passed as an argument</returns>
        public static IVariable Set<TConstraintType>(this IVariable variable, IVariable right) where TConstraintType : Abstraction.Constraint
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.MilpManager.Set<TConstraintType>(variable, right);
		}

        /// <summary>
        /// Adds constraint to a solver
        /// </summary>
        /// <param name="variable">Variable to constrain</param>
        /// <param name="constraint">Constraint type</param>
        /// <param name="right">Right hand side of a constraint</param>
        /// <returns>Variable passed as an argument</returns>
        public static IVariable Set(this IVariable variable, Type constraint, IVariable right)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return (IVariable)typeof(VariableExtensions)
				.GetMethod(nameof(Set), new[] { typeof(IVariable), typeof(IVariable) })
				.MakeGenericMethod(constraint)
				.Invoke(null, new object[] { variable, right });
	    }

        /// <summary>
        /// Adds constraint to a solver
        /// </summary>
        /// <param name="variable">Variable to constrain</param>
        /// <param name="constraintType">Constraint type</param>
        /// <param name="right">Right hand side of a constraint</param>
        /// <returns>Variable passed as an argument</returns>
        public static IVariable Set(this IVariable variable, ConstraintType constraintType, IVariable right)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return Set(variable, ConstraintTypeMapper.Map(constraintType), right);
        }

	    /// <summary>
	    /// Adds composite constraint to a solver
	    /// </summary>
	    /// <typeparam name="TCompositeConstraintType">Constraint type</typeparam>
	    /// <param name="variable">Variable to constrain</param>
	    /// <param name="right">Right hand side of a constraint</param>
	    /// <returns>Variable passed as an argument</returns>
	    public static IVariable Set<TCompositeConstraintType>(this IVariable variable, params IVariable[] right) where TCompositeConstraintType : CompositeConstraint
	    {
	        if (variable == null) throw new ArgumentNullException(nameof(variable));
	        return variable.MilpManager.Set<TCompositeConstraintType>(variable, right);
	    }

	    /// <summary>
	    /// Adds composite constraint to a solver
	    /// </summary>
	    /// <param name="variable">Variable to constrain</param>
	    /// <param name="constraint">Constraint type</param>
	    /// <param name="right">Right hand side of a constraint</param>
	    /// <returns>Variable passed as an argument</returns>
	    public static IVariable Set(this IVariable variable, Type constraint, params IVariable[] right)
	    {
	        if (variable == null) throw new ArgumentNullException(nameof(variable));
	        return (IVariable)typeof(VariableExtensions)
	            .GetMethod(nameof(Set), new[] { typeof(IVariable), typeof(IVariable) })
	            .MakeGenericMethod(constraint)
	            .Invoke(null, new object[] { variable, right });
	    }

	    /// <summary>
	    /// Adds composite constraint to a solver
	    /// </summary>
	    /// <param name="variable">Variable to constrain</param>
	    /// <param name="constraint">Constraint type</param>
	    /// <param name="right">Right hand side of a constraint</param>
	    /// <returns>Variable passed as an argument</returns>
	    public static IVariable Set(this IVariable variable, CompositeConstraintType constraint, params IVariable[] right)
	    {
	        if (variable == null) throw new ArgumentNullException(nameof(variable));
	        return Set(variable, CompositeConstraintTypeMapper.Map(constraint), right);
	    }

        /// <summary>
        /// Adds composite constraint to a solver
        /// </summary>
        /// <typeparam name="TCompositeConstraintType">Constraint type</typeparam>
        /// <param name="variable">Variable to constrain</param>
        /// <param name="parameters">Additional constraint parameters</param>
        /// <param name="right">Right hand side of a constraint</param>
        /// <returns>Variable passed as an argument</returns>
        public static IVariable Set<TCompositeConstraintType>(this IVariable variable, ICompositeConstraintParameters parameters, params IVariable[] right) where TCompositeConstraintType : CompositeConstraint
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.MilpManager.Set<TCompositeConstraintType>(parameters, variable, right);
	    }

        /// <summary>
        /// Adds composite constraint to a solver
        /// </summary>
        /// <param name="variable">Variable to constrain</param>
        /// <param name="constraint">Constraint type</param>
        /// <param name="parameters">Additional constraint parameters</param>
        /// <param name="right">Right hand side of a constraint</param>
        /// <returns>Variable passed as an argument</returns>
        public static IVariable Set(this IVariable variable, Type constraint, ICompositeConstraintParameters parameters, params IVariable[] right)
	    {
	        if (variable == null) throw new ArgumentNullException(nameof(variable));
	        return (IVariable)typeof(VariableExtensions)
	            .GetMethod(nameof(Set), new[] { typeof(IVariable), typeof(ICompositeConstraintParameters), typeof(IVariable) })
	            .MakeGenericMethod(constraint)
	            .Invoke(null, new object[] { variable, parameters, right });
        }

	    /// <summary>
	    /// Adds composite constraint to a solver
	    /// </summary>
	    /// <param name="variable">Variable to constrain</param>
	    /// <param name="constraint">Constraint type</param>
	    /// <param name="parameters">Additional constraint parameters</param>
	    /// <param name="right">Right hand side of a constraint</param>
	    /// <returns>Variable passed as an argument</returns>
	    public static IVariable Set(this IVariable variable, CompositeConstraintType constraint, ICompositeConstraintParameters parameters, params IVariable[] right)
	    {
	        if (variable == null) throw new ArgumentNullException(nameof(variable));
	        return Set(variable, CompositeConstraintTypeMapper.Map(constraint), parameters, right);
	    }

        /// <summary>
        /// Makes goal
        /// </summary>
        /// <typeparam name="TGoalType">Goal type</typeparam>
        /// <param name="variable">Variable to make goal</param>
        /// <param name="variables">Additional variables required to make a goal</param>
        /// <returns>Variable representing goal</returns>
        public static IVariable MakeGoal<TGoalType>(this IVariable variable, params IVariable[] variables) where TGoalType : Goal
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.MilpManager.MakeGoal<TGoalType>(new[] { variable }.Concat(variables).ToArray());
	    }

        /// <summary>
        /// Makes goal
        /// </summary>
        /// <param name="variable">Variable to make goal</param>
        /// <param name="goal">Goal type</param>
        /// <param name="variables">Additional variables required to make a goal</param>
        /// <returns>Variable representing goal</returns>
        public static IVariable MakeGoal(this IVariable variable, Type goal, params IVariable[] variables)
        {
            if (variable == null) throw new ArgumentNullException(nameof(variable));
            return (IVariable)typeof(VariableExtensions)
                .GetMethod(nameof(MakeGoal), new[] { typeof(IVariable), typeof(IVariable) })
                .MakeGenericMethod(typeof(Goal))
                .Invoke(null, new object[] { variable, variables });
        }

	    /// <summary>
	    /// Makes goal
	    /// </summary>
	    /// <param name="variable">Variable to make goal</param>
	    /// <param name="goal">Goal type</param>
	    /// <param name="variables">Additional variables required to make a goal</param>
	    /// <returns>Variable representing goal</returns>
	    public static IVariable MakeGoal(this IVariable variable, GoalType goal, params IVariable[] variables)
	    {
	        if (variable == null) throw new ArgumentNullException(nameof(variable));
	        return MakeGoal(variable, GoalTypeMapper.Map(goal), variables);
	    }

        /// <summary>
        /// Returns value of a variable in solved model
        /// </summary>
        /// <param name="variable">Variable to obtain value</param>
        /// <returns>Value of a variable</returns>
        public static double GetValue(this IVariable variable)
		{
			if (!(variable.MilpManager is IMilpSolver))
			{
				throw new InvalidOperationException("MilpManager doesn't allow to obtain values");
			}
			return ((IMilpSolver) variable.MilpManager).GetValue(variable);
		}

		/// <summary>
		/// Creates new variable equal to passed variable and with adjusted domain
		/// </summary>
		/// <param name="variable">Original variable</param>
		/// <param name="newDomain">Domain of a new variable</param>
		/// <returns>Variable with modified domain</returns>
		public static IVariable ChangeDomain(this IVariable variable, Domain newDomain)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			var newVariable = variable.MilpManager.CreateAnonymous(newDomain);
			variable.Set<Equal>(newVariable);

			return newVariable;
		}

		/// <summary>
		/// Indicates whether variable's domain represents real number
		/// </summary>
		/// <param name="variable">Variable to examine</param>
		/// <returns>True when variable's domain represents real number</returns>
		public static bool IsReal(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.Domain.IsReal();
		}

		/// <summary>
		/// Indicates whether variable's domain represents integer value
		/// </summary>
		/// <param name="variable">Variable to examine</param>
		/// <returns>True when variable's domain represents integer value</returns>
		public static bool IsInteger(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.Domain.IsInteger();
		}

		/// <summary>
		/// Indicates whether variable's domain represents constant value
		/// </summary>
		/// <param name="variable">Variable to examine</param>
		/// <returns>True when variable's domain represents constant value</returns>
		public static bool IsConstant(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.Domain.IsConstant();
		}

		/// <summary>
		/// Indicates whether variable's domain represents non-constant value
		/// </summary>
		/// <param name="variable">Variable to examine</param>
		/// <returns>True when variable's domain represents non-constant value</returns>
		public static bool IsNotConstant(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.Domain.IsNotConstant();
		}

		/// <summary>
		/// Indicates whether variable's domain represents binary value
		/// </summary>
		/// <param name="variable">Variable to examine</param>
		/// <returns>True when variable's domain represent's binary value</returns>
		public static bool IsBinary(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.Domain.IsBinary();
		}

		/// <summary>
		/// Indicates whether variable's domain represents positive or zero value (and not binary one)
		/// </summary>
		/// <param name="variable">Variable to examine</param>
		/// <returns>True when variable's domain represents positive or zero value (and not binary one)</returns>
		public static bool IsPositiveOrZero(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.Domain.IsPositiveOrZero();
		}

		/// <summary>
		/// Indicates whether variable's domain represents non-negative value (binary || positive or zero)
		/// </summary>
		/// <param name="variable">Variable to examine</param>
		/// <returns>True when variable's domain represents non-negative value (binary || positive or zero)</returns>
		public static bool IsNonNegative(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.Domain.IsNonNegative();
		}

		/// <summary>
		/// Textual representation of a variable
		/// </summary>
		/// <param name="variable">Variable to obtain representation</param>
		/// <returns>Textual representation of a variable</returns>
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

		/// <summary>
		/// Adds constraint to make variable equal to one
		/// </summary>
		/// <param name="variable">Variable to constrain</param>
		/// <returns>Result of constraint addition</returns>
		public static IVariable MakeTrue(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.Set<Equal>(variable.MilpManager.FromConstant(1));
		}


		/// <summary>
		/// Adds constraint to make variable equal to zero
		/// </summary>
		/// <param name="variable">Variable to constrain</param>
		/// <returns>Result of constraint addition</returns>
		public static IVariable MakeFalse(this IVariable variable)
		{
			if (variable == null) throw new ArgumentNullException(nameof(variable));
			return variable.Set<Equal>(variable.MilpManager.FromConstant(0));
		}

		/// <summary>
		/// Calculate lowest domain capable of holding values of both variables
		/// </summary>
		/// <param name="first">First variable to consider</param>
		/// <param name="second">Second variable to consider</param>
		/// <returns>Lowest domain capable of holding both variables</returns>
		public static Domain LowestEncompassingDomain(this IVariable first, IVariable second)
		{
			if (first == null) throw new ArgumentNullException(nameof(first));
			if (second == null) throw new ArgumentNullException(nameof(second));

			return first.Domain.LowestEncompassingDomain(second.Domain);
		}
	}
}
