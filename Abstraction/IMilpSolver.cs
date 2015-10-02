using System.IO;

namespace MilpManager.Abstraction
{
    public interface IMilpSolver : IMilpManager
    {
        void AddGoal(string name, IVariable operation);
        string GetGoalExpression(string name);
        void SaveModelToFile(string modelPath);
        void LoadModel(string modelPath, Stream solverData);
        void SaveSolverData(Stream solverData);
        IVariable GetByName(string name);
        IVariable TryGetByName(string name);
        void Solve();
        double GetValue(IVariable variable);
        SolutionStatus GetStatus();
    }
}