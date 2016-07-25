using System.IO;

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
        /// Saves created model to file
        /// </summary>
        /// <param name="modelPath">File path</param>
        /// <remarks>Format of a file is implementation dependent, usually based on a file extension</remarks>
        void SaveModelToFile(string modelPath);
        /// <summary>
        /// Loads model and solver's internal data so it is possible to resume working on a model
        /// </summary>
        /// <param name="modelPath">Model file path</param>
        /// <param name="solverData">Stream representing solver data</param>
        void LoadModel(string modelPath, Stream solverData);
        /// <summary>
        /// Saves solver's internal data
        /// </summary>
        /// <param name="solverData">Stream to store solver's data</param>
        void SaveSolverData(Stream solverData);
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