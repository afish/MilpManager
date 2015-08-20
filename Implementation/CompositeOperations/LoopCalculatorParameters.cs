using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class LoopCalculatorParameters : ICompositeOperationParameters
    {
        public int MaxIterations { get; set; }
        public Func<IVariable, IVariable[], IVariable>[] Body { get; set; }
    }
}