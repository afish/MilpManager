using System;
using System.Collections.Generic;
using System.Linq;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.CompositeOperations
{
	public abstract class BaseCompositeOperationCalculator : ICompositeOperationCalculator
	{
		public bool SupportsOperation<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments) where TCompositeOperationType : CompositeOperationType
		{
			return SupportedTypes.Contains(typeof(TCompositeOperationType)) && SupportsOperationInternal<TCompositeOperationType>(parameters, arguments);
		}

		public IEnumerable<IVariable> Calculate<TCompositeOperationType>(IMilpManager milpManager, ICompositeOperationParameters parameters,
			params IVariable[] arguments) where TCompositeOperationType : CompositeOperationType
		{
			if (!SupportsOperation<TCompositeOperationType>(parameters, arguments)) throw new NotSupportedException(SolverUtilities.FormatUnsupportedMessage(typeof(TCompositeOperationType), parameters, arguments));

			if (IsConstantOperation<TCompositeOperationType>(parameters, arguments))
			{
				return CalculateConstantInternal<TCompositeOperationType>(milpManager, parameters, arguments);
			}
			else
			{
				var result = CalculateInternal<TCompositeOperationType>(milpManager, parameters, arguments);

				return result;
			}
		}

		protected virtual bool IsConstantOperation<TCompositeOperationType>(ICompositeOperationParameters parameters,
			params IVariable[] arguments) where TCompositeOperationType : CompositeOperationType
		{
			return arguments.All(x => x.IsConstant());
		}

		protected abstract bool SupportsOperationInternal<TCompositeOperationType>(ICompositeOperationParameters parameters, params IVariable[] arguments)
			where TCompositeOperationType : CompositeOperationType;

		protected abstract IEnumerable<IVariable> CalculateInternal<TCompositeOperationType>(IMilpManager milpManager, ICompositeOperationParameters parameters, params IVariable[] arguments)
			where TCompositeOperationType : CompositeOperationType;

		protected abstract IEnumerable<IVariable> CalculateConstantInternal<TCompositeOperationType>(IMilpManager milpManager, ICompositeOperationParameters parameters, params IVariable[] arguments)
			where TCompositeOperationType : CompositeOperationType;

		protected abstract Type[] SupportedTypes { get; }
	}
}