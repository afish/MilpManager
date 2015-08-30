using System.Collections.Generic;

namespace MilpManager.Abstraction
{
    public interface IMilpManager
    {
        int IntegerWidth { get; }
        int IntegerInfinity { get; }
        int MaximumIntegerValue { get; }
        IVariable Create(string name, IVariable value);
        IVariable Create(IVariable value);
        IVariable Operation(OperationType type, params IVariable[] variables);
        IVariable Set(ConstraintType type, IVariable left, IVariable right);

        IEnumerable<IVariable> CompositeOperation(CompositeOperationType type,
            params IVariable[] variables);

        IEnumerable<IVariable> CompositeOperation(CompositeOperationType type,
            ICompositeOperationParameters parameters, params IVariable[] variables);

        IVariable Set(CompositeConstraintType type, IVariable left,
            params IVariable[] variables);

        IVariable Set(CompositeConstraintType type, ICompositeConstraintParameters parameters, IVariable left,
            params IVariable[] variables);
        IVariable SumVariables(IVariable first, IVariable second, Domain domain);
        IVariable NegateVariable(IVariable variable, Domain domain);
        IVariable MultiplyVariableByConstant(IVariable variable, IVariable constant, Domain domain);
        IVariable DivideVariableByConstant(IVariable variable, IVariable constant, Domain domain);
        void SetLessOrEqual(IVariable variable, IVariable bound);
        void SetGreaterOrEqual(IVariable variable, IVariable bound);
        void SetEqual(IVariable variable, IVariable bound);
        IVariable FromConstant(int value);
        IVariable FromConstant(int value, Domain domain);
        IVariable FromConstant(double value, Domain domain);
        IVariable FromConstant(double value);
        IVariable Create(string name, Domain domain);
        IVariable CreateAnonymous(Domain domain);
    }
}