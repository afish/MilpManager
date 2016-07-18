using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class Approximate2DParameters : ICompositeOperationParameters
    {
        public double[] ArgumentsX { get; set; }
        public double[] ArgumentsY { get; set; }
        public Func<double, double, double> Function { get; set; }
        public string FunctionDescription { get; set; }
        public bool ArgumentMustBeOnAGrid { get; set; }

        public Approximate2DParameters()
        {
            FunctionDescription = "<anonymous_function>";
            ArgumentMustBeOnAGrid = false;
        }
    }
}