namespace MilpManager.Abstraction
{
    public abstract class BaseMilpSolver : BaseMilpManager, IMilpSolver
    {
        public BaseMilpSolver(int integerWidth) : base(integerWidth)
        {
        }
        public BaseMilpSolver()
        {
        }

        public abstract void AddGoal(string name, IVariable operation);
        public abstract string GetGoalExpression(string name);
        public abstract void SaveModelToFile(string modelPath);
        public abstract void LoadModelFromFile(string modelPath, string solverDataPath);
        public abstract void SaveSolverDataToFile(string solverOutput);
        public abstract IVariable GetByName(string name);
        public abstract IVariable TryGetByName(string name);
        public abstract void Solve();
        public abstract double GetValue(IVariable variable);
        public abstract SolutionStatus GetStatus();
    }
}