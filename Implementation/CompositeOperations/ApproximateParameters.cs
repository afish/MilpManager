using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class ApproximateParameters : ICompositeOperationParameters
    {
        public double[] Arguments { get; set; }
        public Func<double, double> Function { get; set; }
        public string FunctionDescription { get; set; }
        public bool ArgumentMustBeOnAGrid { get; set; }

        public ApproximateParameters()
        {
            FunctionDescription = "<anonymous_function>";
            ArgumentMustBeOnAGrid = false;
        }
    }
}