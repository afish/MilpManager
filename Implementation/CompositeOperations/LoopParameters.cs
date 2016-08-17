using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class LoopParameters : ICompositeOperationParameters
    {
        public int MaxIterations { get; set; }
        public Func<IVariable, IVariable, IVariable, IVariable, IVariable[], IVariable>[] Body { get; set; }
        public Action<IVariable, IVariable, IVariable, IVariable, IVariable[]>[] BeforeBody { get; set; }
        public Action<IVariable, IVariable, IVariable, IVariable, IVariable[]>[] AfterBody { get; set; }
        public Action<IVariable, IVariable, IVariable, IVariable[]> BeforeIterationAction { get; set; }
        public Action<IVariable, IVariable, IVariable, IVariable[]> AfterIterationAction { get; set; }
        public Action<IVariable, IVariable[]> BeforeLoopAction { get; set; }
        public Action<IVariable, IVariable[]> AfterLoopAction { get; set; }

        public LoopParameters()
        {
            BeforeLoopAction = (totalBound, arguments) => { };
            AfterLoopAction = (totalBound, arguments) => { };
            BeforeIterationAction = (counter, isLooping, totalBound, arguments) => { };
            AfterIterationAction = (counter, isLooping, totalBound, arguments) => { };
            BeforeBody = new Action<IVariable, IVariable, IVariable, IVariable, IVariable[]>[0];
            AfterBody = new Action<IVariable, IVariable, IVariable, IVariable, IVariable[]>[0];
        }
    }
}