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
        public IVariable GetByName(string name)
        {
            return Variables[name];
        }

        public IVariable TryGetByName(string name)
        {
            IVariable variable;
            return Variables.TryGetValue(name, out variable) ? variable : null;
        }

        public override sealed IVariable Create(string name, Domain domain)
        {
            var variable = InternalCreate(name, domain);
            Variables[name] = variable;
            return variable;
        }

        public override sealed IVariable CreateAnonymous(Domain domain)
        {
            return Create(NewVariableName(), domain);
        }

        public override sealed IVariable FromConstant(int value, Domain domain)
        {
            var variableName = NewVariableName();
            var variable = InternalFromConstant(variableName, value, domain);
            Variables[variableName] = variable;
            variable.ConstantValue = value;
            return variable;
        }

        public override sealed IVariable FromConstant(double value, Domain domain)
        {
            var variableName = NewVariableName();
            var variable = InternalFromConstant(variableName, value, domain);
            Variables[variableName] = variable;
            variable.ConstantValue = value;
            return variable;
        }
        public override sealed IVariable SumVariables(IVariable first, IVariable second, Domain domain)
        {
            var newVariable = InternalSumVariables(first, second, domain);
            newVariable.ConstantValue = first.ConstantValue + second.ConstantValue;
            return newVariable;
        }

        public override sealed IVariable NegateVariable(IVariable variable, Domain domain)
        {
            var newVariable = InternalNegateVariable(variable, domain);
            newVariable.ConstantValue = -variable.ConstantValue;
            return newVariable;
        }

        public override sealed IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            var newVariable = InternalMultiplyVariableByConstant(variable, constant, domain);
            newVariable.ConstantValue = variable.ConstantValue * constant.ConstantValue;
            return newVariable;
        }

        public override sealed IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain)
        {
            var newVariable = InternalDivideVariableByConstant(variable, constant, domain);
            newVariable.ConstantValue = variable.ConstantValue / constant.ConstantValue;
            return newVariable;
        }

        public override sealed void SetEqual(IVariable variable, IVariable bound)
        {
            InternalSetEqual(variable, bound);
            variable.ConstantValue = bound.ConstantValue ?? variable.ConstantValue;
            bound.ConstantValue = variable.ConstantValue ?? bound.ConstantValue;
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
        protected virtual void InternalSetEqual(IVariable variable, IVariable bound)
        {
            base.SetEqual(variable, bound);
        }
        protected string NewVariableName()
        {
            return $"_v_{VariableIndex++}";
        }
    }
}