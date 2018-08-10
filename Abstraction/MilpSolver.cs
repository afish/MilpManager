using System.Collections.Generic;

namespace MilpManager.Abstraction
{
    public abstract class MilpSolver : MilpManager, IMilpSolver
    {
        protected IDictionary<string, IVariable> Goals;

        protected MilpSolver(MilpManagerSettings settings) : base(settings)
        {
            Goals = new Dictionary<string, IVariable>();
        }

        public virtual void AddGoal(string name, IVariable operation)
        {
            Goals[name] = operation;
            InternalAddGoal(name, operation);
        }

        public virtual string GetGoalExpression(string name)
        {
            return Goals[name].Expression;
        }

        public virtual IVariable GetByName(string name)
        {
            return Variables[name];
        }

        public virtual IVariable TryGetByName(string name)
        {
            IVariable variable;
            return Variables.TryGetValue(name, out variable) ? variable : null;
        }

        public abstract void Solve();
        public abstract double GetValue(IVariable variable);
        public abstract SolutionStatus GetStatus();
        protected abstract void InternalAddGoal(string name, IVariable operation);
    }
}
