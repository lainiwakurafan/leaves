﻿namespace AbcLeaves.Core
{
    public class OperationFlowState<TReturn, TCurrent>
        where TReturn : IOperationResult
        where TCurrent : IOperationResult
    {
        public OperationFlowState()
        {
        }

        public OperationFlowState(TCurrent currentResult)
        {
            Current = currentResult;
        }

        public OperationFlowState(TReturn returnResult)
        {
            Return = returnResult;
        }

        public TReturn Return { get; set; }
        public TCurrent Current { get; set; }
    }
}
