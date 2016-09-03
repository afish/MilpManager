using System.Collections.Generic;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
    public class CountingSortParameters : ICompositeOperationParameters
    {
         public IVariable[] Values { get; set; }
    }
}