using System.Collections.Generic;

namespace MilpManager.Abstraction
{
    public abstract class BaseMilpSolver : BaseMilpManager, IMilpSolver
    {
        protected IDictionary<string, IVariable> Variables;
        protected int VariableIndex;

        protected BaseMilpSolver(int integerWidth) : base(integerWidth)
        {
            Variables = new Dictionary<string, IVariable>();
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

        public override IVariable Create(string name, Domain domain)
        {
            var variable = InternalCreate(name, domain);
            Variables[name] = variable;
            return variable;
        }

        public override IVariable CreateAnonymous(Domain domain)
        {
            return Create(NewVariableName(), domain);
        }

        public override IVariable FromConstant(int value, Domain domain)
        {
            var variableName = NewVariableName();
            var variable = InternalFromConstant(variableName, value, domain);
            Variables[variableName] = variable;
            variable.ConstantValue = value;
            return variable;
        }

        public override IVariable FromConstant(double value, Domain domain)
        {
            var variableName = NewVariableName();
            var variable = InternalFromConstant(variableName, value, domain);
            Variables[variableName] = variable;
            variable.ConstantValue = value;
            return variable;
        }
        public override IVariable SumVariables(IVariable first, IVariable second, Domain domain)
        {
            var newVariable = InternalSumVariables(first, second, domain);
            Variables[newVariable.Name] = newVariable;
            return newVariable;
        }

        public override IVariable NegateVariable(IVariable variable, Domain domain)
        {
            var newVariable = InternalNegateVariable(variable, domain);
            Variables[newVariable.Name] = newVariable;
            return newVariable;
        }

        public override IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            var newVariable = InternalMultiplyVariableByConstant(variable, constant, domain);
            Variables[newVariable.Name] = newVariable;
            return newVariable;
        }

        public override IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            var newVariable = InternalDivideVariableByConstant(variable, constant, domain);
            Variables[newVariable.Name] = newVariable;
            return newVariable;
        }

        public abstract void AddGoal(string name, IVariable operation);
        public abstract string GetGoalExpression(string name);
        public abstract void SaveModelToFile(string modelPath);
        public abstract void LoadModelFromFile(string modelPath, string solverDataPath);
        public abstract void SaveSolverDataToFile(string solverOutput);
        public abstract void Solve();
        public abstract double GetValue(IVariable variable);
        public abstract SolutionStatus GetStatus();
        protected abstract IVariable InternalCreate(string name, Domain domain);
        protected abstract IVariable InternalFromConstant(string name, int value, Domain domain);
        protected abstract IVariable InternalFromConstant(string name, double value, Domain domain);
        protected abstract IVariable InternalSumVariables(IVariable first, IVariable second, Domain domain);
        protected abstract IVariable InternalNegateVariable(IVariable variable, Domain domain);
        protected abstract IVariable InternalMultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain);
        protected abstract IVariable InternalDivideVariableByConstant(IVariable variable, IVariable constant,Domain domain);
        protected virtual string NewVariableName()
        {
            return $"_v_{VariableIndex++}";
        }
    }
}