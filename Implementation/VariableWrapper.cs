using MilpManager.Abstraction;

namespace MilpManager.Implementation
{
    public class VariableWrapper : IVariable
    {
        public IMilpManager MilpManager
        {
            get { return Wrapped.MilpManager; }
            set { Wrapped.MilpManager = value; }
        }

        public Domain Domain
        {
            get { return Wrapped.Domain; }
            set { Wrapped.Domain = value; }
        }

        public string Name
        {
            get { return Wrapped.Name; }
            set { Wrapped.Name = value; }
        }

        public double? ConstantValue
        {
            get { return Wrapped.ConstantValue; }
            set { Wrapped.ConstantValue = value; }
        }

        public string Expression
        {
            get { return Wrapped.Expression; }
            set { Wrapped.Expression = value; }
        }

        public IVariable Wrapped { get; }

        public VariableWrapper(IVariable wrapped)
        {
            Wrapped = wrapped;
        }

        public static VariableWrapper operator +(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Addition, b).Wrap();
        }

        public static VariableWrapper operator -(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Subtraction, b).Wrap();
        }

        public static VariableWrapper operator *(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Multiplication, b).Wrap();
        }

        public static VariableWrapper operator /(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Division, b).Wrap();
        }

        public static VariableWrapper operator %(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Remainder, b).Wrap();
        }

        public static VariableWrapper operator ^(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.ExclusiveDisjunction, b).Wrap();
        }

        public static VariableWrapper operator |(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Disjunction, b).Wrap();
        }

        public static VariableWrapper operator &(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Conjunction, b).Wrap();
        }
    }
}