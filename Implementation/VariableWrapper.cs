using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation
{
    public class VariableWrapper
    {
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
        /// Performs addition
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of addition</returns>
        public static VariableWrapper operator +(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.Addition, b.Wrapped).Wrap();
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
        /// Performs subtraction
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of subtraction</returns>
        public static VariableWrapper operator -(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.Subtraction, b.Wrapped).Wrap();
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
        /// Performs multiplication
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of multiplication</returns>
        public static VariableWrapper operator *(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.Multiplication, b.Wrapped).Wrap();
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
        /// Performs division
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of division</returns>
        public static VariableWrapper operator /(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.Division, b.Wrapped).Wrap();
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
        /// Performs modulo
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of modulo</returns>
        public static VariableWrapper operator %(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.Remainder, b.Wrapped).Wrap();
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
        /// Performs exclusive disjunction (XOR)
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of exclusive disjunction (XOR)</returns>
        public static VariableWrapper operator ^(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.ExclusiveDisjunction, b.Wrapped).Wrap();
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
        /// Performs disjunction
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of disjunction</returns>
        public static VariableWrapper operator |(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.Disjunction, b.Wrapped).Wrap();
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

        /// <summary>
        /// Performs conjunction
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of conjunction</returns>
        public static VariableWrapper operator &(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.Conjunction, b.Wrapped).Wrap();
        }
        
        /// <summary>
        /// Performs LT comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of LT comparison</returns>
        public static VariableWrapper operator <(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.IsLessThan, b).Wrap();
        }

        /// <summary>
        /// Performs LT comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of LT comparison</returns>
        public static VariableWrapper operator <(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.IsLessThan, b.Wrapped).Wrap();
        }

        /// <summary>
        /// Performs GT comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of GT comparison</returns>
        public static VariableWrapper operator >(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.IsGreaterThan, b).Wrap();
        }

        /// <summary>
        /// Performs GT comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of GT comparison</returns>
        public static VariableWrapper operator >(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.IsGreaterThan, b.Wrapped).Wrap();
        }

        /// <summary>
        /// Performs LE comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of LE comparison</returns>
        public static VariableWrapper operator <=(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.IsLessOrEqual, b).Wrap();
        }

        /// <summary>
        /// Performs LE comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of LE comparison</returns>
        public static VariableWrapper operator <=(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.IsLessOrEqual, b.Wrapped).Wrap();
        }

        /// <summary>
        /// Performs GE comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of GE comparison</returns>
        public static VariableWrapper operator >=(VariableWrapper a, IVariable b)
        {
            return a.Wrapped.Operation(OperationType.IsGreaterOrEqual, b).Wrap();
        }

        /// <summary>
        /// Performs GE comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of GE comparison</returns>
        public static VariableWrapper operator >=(VariableWrapper a, VariableWrapper b)
        {
            return a.Wrapped.Operation(OperationType.IsGreaterOrEqual, b.Wrapped).Wrap();
        }

        /// <summary>
        /// Performs EQ comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of EQ comparison</returns>
        public static VariableWrapper operator ==(VariableWrapper a, IVariable b)
        {
            if ((object) a == null)
            {
                throw new ArgumentException(nameof(a));
            }
            return a.Wrapped.Operation(OperationType.IsEqual, b).Wrap();
        }

        /// <summary>
        /// Performs EQ comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of EQ comparison</returns>
        public static VariableWrapper operator ==(VariableWrapper a, VariableWrapper b)
        {
            if ((object) a == null)
            {
                throw new ArgumentException(nameof(a));
            }
            return a.Wrapped.Operation(OperationType.IsEqual, b.Wrapped).Wrap();
        }

        /// <summary>
        /// Performs NEQ comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of NEQ comparison</returns>
        public static VariableWrapper operator !=(VariableWrapper a, IVariable b)
        {
            if ((object)a == null)
            {
                throw new ArgumentException(nameof(a));
            }
            return a.Wrapped.Operation(OperationType.IsNotEqual, b).Wrap();
        }

        /// <summary>
        /// Performs NEQ comparison
        /// </summary>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <returns>Result of NEQ comparison</returns>
        public static VariableWrapper operator !=(VariableWrapper a, VariableWrapper b)
        {
            if ((object)a == null)
            {
                throw new ArgumentException(nameof(a));
            }
            return a.Wrapped.Operation(OperationType.IsNotEqual, b.Wrapped).Wrap();
        }
    }
}