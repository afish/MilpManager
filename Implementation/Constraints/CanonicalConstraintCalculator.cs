﻿using System;
using MilpManager.Abstraction;

namespace MilpManager.Implementation.Constraints
{
    public class CanonicalConstraintCalculator : IConstraintCalculator
    {
        public IVariable Set(IMilpManager milpManager, ConstraintType type, IVariable leftVariable,
            IVariable rightVariable)
        {
            switch (type)
            {
                case ConstraintType.Equal:
                    milpManager.SetEqual(leftVariable, rightVariable);
                    leftVariable.ConstantValue = rightVariable.ConstantValue ?? leftVariable.ConstantValue;
                    rightVariable.ConstantValue = leftVariable.ConstantValue ?? rightVariable.ConstantValue;
                    break;
                case ConstraintType.LessOrEqual:
                    milpManager.SetLessOrEqual(leftVariable, rightVariable);
                    break;
                case ConstraintType.GreaterOrEqual:
                    milpManager.SetGreaterOrEqual(leftVariable, rightVariable);
                    break;
                case ConstraintType.LessThan:
                    milpManager.Operation(OperationType.IsLessThan, leftVariable, rightVariable)
                        .Set(ConstraintType.Equal, milpManager.FromConstant(1));
                    break;
                case ConstraintType.GreaterThan:
                    milpManager.Operation(OperationType.IsGreaterThan, leftVariable, rightVariable)
                        .Set(ConstraintType.Equal, milpManager.FromConstant(1));
                    break;
                case ConstraintType.NotEqual:
                    milpManager.Operation(OperationType.IsNotEqual, leftVariable, rightVariable)
                        .Set(ConstraintType.Equal, milpManager.FromConstant(1));
                    break;
                default:
                    throw new InvalidOperationException("Cannot set constraint");
            }
            return leftVariable;
        }
    }
}