using System.Collections.Generic;

namespace MilpManager.Abstraction
{
	public interface IMilpManager
	{
		/// <summary>
		///  Size of integer in bits
		/// </summary>
		int IntegerWidth { get; }
		/// <summary>
		///  Positive value greater than any supported integer value, should be at least 4*MaximumIntegerValue + 1
		/// </summary>
		int IntegerInfinity { get; }
		/// <summary>
		///  Maximum supported positive integer value
		/// </summary>
		int MaximumIntegerValue { get; }
		/// <summary>
		///  Precision used when working with real numbers
		/// </summary>
		double Epsilon { get; }
		/// <summary>
		///  Creates variable based on existing value
		/// </summary>
		/// <param name="name">Name of created variable</param>
		/// <param name="value">Value used to create variable</param>
		/// <returns>Created variable</returns>
		IVariable Create(string name, IVariable value);
		/// <summary>
		///  Creates anonymous variable based on existing value
		/// </summary>
		/// <param name="value">Value used to create variable</param>
		/// <returns>Created variable</returns>
		IVariable Create(IVariable value);
		/// <summary>
		///  Performs operation on passed arguments
		/// </summary>
		/// <typeparam name="TOperationType">Type of operation to perform</typeparam>
		/// <param name="variables">Operation arguments</param>
		/// <returns>Result of operation</returns>
		IVariable Operation<TOperationType>(params IVariable[] variables) where TOperationType : OperationType;
		/// <summary>
		///  Adds constraint for left hand side using value of right hand side
		/// </summary>
		/// <param name="left">Variable to set</param>
		/// <param name="right">Variable representing constraint's right hand side</param>
		/// <param name="type">Constraint type</param>
		/// <returns>Returns left</returns>
		IVariable Set(ConstraintType type, IVariable left, IVariable right);
		/// <summary>
		///  Performs composite operation on passed arguments
		/// </summary>
		/// <param name="type">Type of operation to perform</param>
		/// <param name="variables">Operation arguments</param>
		/// <returns>Sequence of results of operation</returns>
		IEnumerable<IVariable> CompositeOperation(CompositeOperationType type, params IVariable[] variables);
		/// <summary>
		///  Performs composite operation on passed arguments
		/// </summary>
		/// <param name="type">Type of operation to perform</param>
		/// <param name="parameters">Additional operation parameters (depends on the type of operation)</param>
		/// <param name="variables">Operation arguments</param>
		/// <returns>Sequence of results of operation</returns>
		IEnumerable<IVariable> CompositeOperation(CompositeOperationType type, ICompositeOperationParameters parameters, params IVariable[] variables);
		/// <summary>
		///  Adds constraint for left hand side using values of right hand side
		/// </summary>
		/// <param name="type">Constraint type</param>
		/// <param name="left">Variable to set</param>
		/// <param name="variables">Variables to use as a right hand side</param>
		/// <returns>Returns left</returns>
		/// <remarks>Some constraints work on a sequence of variables, not on a left hand side directly. E.g., setting variables to AllDifferent.</remarks>
		IVariable Set(CompositeConstraintType type, IVariable left, params IVariable[] variables);
		/// <summary>
		///  Adds constraint for left hand side using values of right hand side
		/// </summary>
		/// <param name="type">Constraint type</param>
		/// <param name="left">Variable to set</param>
		/// <param name="variables">Variables to use as a right hand side</param>
		/// <param name="parameters">Additional constraint parameters (depends on the type of operation)</param>
		/// <returns>Returns left</returns>
		/// <remarks>Some constraints work on a sequence of variables, not on a left hand side directly. E.g., setting variables to AllDifferent.</remarks>
		IVariable Set(CompositeConstraintType type, ICompositeConstraintParameters parameters, IVariable left, params IVariable[] variables);
		/// <summary>
		///  Makes goal based on variables. The goal is not added to solver
		/// </summary>
		/// <param name="type">Goal type</param>
		/// <param name="variables">Variables to create goal from</param>
		/// <returns>Variable representing goal</returns>
		IVariable MakeGoal(GoalType type, params IVariable[] variables);
		/// <summary>
		/// Calculates sum of variables.
		/// </summary>
		/// <param name="first">First variable to sum</param>
		/// <param name="second">Second variable to sum</param>
		/// <param name="domain">Domain of a result</param>
		/// <returns>Variable representing sum of arguments</returns>
		IVariable SumVariables(IVariable first, IVariable second, Domain domain);
		/// <summary>
		/// Calculates negation of a variable
		/// </summary>
		/// <param name="variable">Variable to negate</param>
		/// <param name="domain">Domain of a result</param>
		/// <returns>Variable representing negation of an argument</returns>
		IVariable NegateVariable(IVariable variable, Domain domain);
		/// <summary>
		/// Calculates multiplication of a variable and a constant
		/// </summary>
		/// <param name="variable">Variable to multiply</param>
		/// <param name="constant">Variable representing constant to multiply</param>
		/// <param name="domain">Domain of a result</param>
		/// <returns>Variable representing multiplciation of arguments</returns>
		IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain);
		/// <summary>
		/// Calculates quotient of a variable and a constant
		/// </summary>
		/// <param name="variable">Variable to divide</param>
		/// <param name="constant">Variable representing constant to divide</param>
		/// <param name="domain">Domain of a result</param>
		/// <returns>Variable representing quotient of argument</returns>
		IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain);
		/// <summary>
		/// Adds LE constraint
		/// </summary>
		/// <param name="variable">Variable to constrain</param>
		/// <param name="bound">Right hand side of a constraint</param>
		void SetLessOrEqual(IVariable variable, IVariable bound);
		/// <summary>
		/// Adds GE constraint
		/// </summary>
		/// <param name="variable">Variable to constrain</param>
		/// <param name="bound">Right hand side of a constraint</param>
		void SetGreaterOrEqual(IVariable variable, IVariable bound);
		/// <summary>
		/// Adds EQ constraint
		/// </summary>
		/// <param name="variable">Variable to constrain</param>
		/// <param name="bound">Right hand side of a constraint</param>
		void SetEqual(IVariable variable, IVariable bound);
		/// <summary>
		/// Creates variable representing constant
		/// </summary>
		/// <param name="value">Constant value for variable</param>
		/// <returns>Variable representing constant</returns>
		IVariable FromConstant(int value);
		/// <summary>
		/// Creates variable representing constant with specified domain
		/// </summary>
		/// <param name="value">Constant value for variable</param>
		/// <param name="domain">Domain for variable</param>
		/// <returns>Variable representing constant</returns>
		IVariable FromConstant(int value, Domain domain);
		/// <summary>
		/// Creates variable representing constant with specified domain
		/// </summary>
		/// <param name="value">Constant value for variable</param>
		/// <param name="domain">Domain for variable</param>
		/// <returns>Variable representing constant</returns>
		IVariable FromConstant(double value, Domain domain);
		/// <summary>
		/// Creates variable representing constant
		/// </summary>
		/// <param name="value">Constant value for variable</param>
		/// <returns>Variable representing constant</returns>
		IVariable FromConstant(double value);
		/// <summary>
		/// Creates variable with specified name and domain
		/// </summary>
		/// <param name="name">Name of a variable</param>
		/// <param name="domain">Domain of a variable</param>
		/// <returns>Variable with specified name and domain</returns>
		IVariable Create(string name, Domain domain);
		/// <summary>
		/// Creates anonymous (with implementation dependent name) variable with specified domain
		/// </summary>
		/// <param name="domain">Domain of a variable</param>
		/// <returns>Variable with specified domain</returns>
		IVariable CreateAnonymous(Domain domain);
	}
}