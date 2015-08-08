using System.Collections.Generic;

namespace MilpManager.Abstraction
{
    public interface IMilpSolver : IMilpManager
    {
        void AddGoal(string name, IVariable operation);
        string GetGoalExpression(string name);
        void SaveModelToFile(string modelPath);
        void LoadModelFromFile(string modelPath, string solverDataPath);
        void SaveSolverDataToFile(string solverOutput);
        IVariable GetByName(string name);
        IVariable TryGetByName(string name);
        void Solve();
        double GetValue(IVariable variable);
        SolutionStatus GetStatus();
    }
}