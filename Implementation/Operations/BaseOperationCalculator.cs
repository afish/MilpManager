using System;
using System.Linq;
using MilpManager.Abstraction;
using MilpManager.Utilities;
using Operation = MilpManager.Abstraction.Operation;

namespace MilpManager.Implementation.Operations
{
	public abstract class BaseOperationCalculator : IOperationCalculator
	{
		public bool SupportsOperation<TOperationType>(params IVariable[] arguments) where TOperationType : Operation
		{
			return SupportedTypes.Contains(typeof (TOperationType)) && SupportsOperationInternal<TOperationType>(arguments);
		}

		public IVariable Calculate<TOperationType>(IMilpManager milpManager, params IVariable[] arguments) where TOperationType : Operation
		{
			if (!SupportsOperation<TOperationType>(arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(typeof(TOperationType), arguments));

			if (arguments.All(x => x.IsConstant()))
			{
				return CalculateConstantInternal<TOperationType>(milpManager, arguments);
			}
			else
			{
				var result = CalculateInternal<TOperationType>(milpManager, arguments);

				return result;
			}
		}

		protected abstract bool SupportsOperationInternal<TOperationType>(params IVariable[] arguments)
			where TOperationType : Operation;

		protected abstract IVariable CalculateInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
			where TOperationType : Operation;

		protected abstract IVariable CalculateConstantInternal<TOperationType>(IMilpManager milpManager, params IVariable[] arguments)
			where TOperationType : Operation;

		protected abstract Type[] SupportedTypes { get; }
	}
}