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

        /// <summary>
        /// Wrapped variable
        /// </summary>
        public IVariable Wrapped { get; }

        public VariableWrapper(IVariable wrapped)
        {
            Wrapped = wrapped;
        }

        /// <summary>
        /// Performs addition
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of addition</returns>
        public static VariableWrapper operator +(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Addition, b).Wrap();
        }

        /// <summary>
        /// Performs subtraction
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of subtraction</returns>
        public static VariableWrapper operator -(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Subtraction, b).Wrap();
        }
        
        /// <summary>
        /// Performs multiplication
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of multiplication</returns>
        public static VariableWrapper operator *(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Multiplication, b).Wrap();
        }
        
        /// <summary>
        /// Performs division
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of division</returns>
        public static VariableWrapper operator /(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Division, b).Wrap();
        }
        
        /// <summary>
        /// Performs modulo
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of modulo</returns>
        public static VariableWrapper operator %(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Remainder, b).Wrap();
        }
        
        /// <summary>
        /// Performs exclusive disjunction (XOR)
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of exclusive disjunction (XOR)</returns>
        public static VariableWrapper operator ^(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.ExclusiveDisjunction, b).Wrap();
        }
        
        /// <summary>
        /// Performs disjunction
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of disjunction</returns>
        public static VariableWrapper operator |(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Disjunction, b).Wrap();
        }
        
        /// <summary>
        /// Performs conjunction
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of conjunction</returns>
        public static VariableWrapper operator &(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.Conjunction, b).Wrap();
        }
    }
}