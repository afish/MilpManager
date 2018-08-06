namespace MilpManager.Abstraction
{
	public interface IMilpSolver : IMilpManager
	{
		/// <summary>
		/// Adds goal to solver
		/// </summary>
		/// <param name="name">Goal name</param>
		/// <param name="operation">Variable representing goal</param>
		void AddGoal(string name, IVariable operation);

		/// <summary>
		/// Returns textual representation of a goal
		/// </summary>
		/// <param name="name">Goal to retrieve representation</param>
		/// <returns>Textual representation of a goal</returns>
		string GetGoalExpression(string name);

		/// <summary>
		/// Returns variable with specified name
		/// </summary>
		/// <param name="name">Variable to retrieve</param>
		/// <returns>Variable with specified name</returns>
		IVariable GetByName(string name);

		/// <summary>
		/// Returns variable with specified name if it exists
		/// </summary>
		/// <param name="name">Variable to retrieve</param>
		/// <returns>Variable with specified name</returns>
		IVariable TryGetByName(string name);

		/// <summary>
		/// Solves model
		/// </summary>
		void Solve();

		/// <summary>
		/// Returns calculated value of a variable
		/// </summary>
		/// <param name="variable">Variable with value</param>
		/// <returns>Calculated variable value</returns>
		double GetValue(IVariable variable);

		/// <summary>
		/// Returns status of a solution
		/// </summary>
		/// <returns>Status of a solution</returns>
		SolutionStatus GetStatus();
	}
}