using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPlayground
{
    interface ITaskPool { }
    class PooledTask<TDo, TCallback>
    {
        TDo @do;
        TCallback callback;
        public PooledTask(TDo @do, TCallback callback)
        {
            this.@do = @do;
            this.callback = callback;
        }
    }
    class PooledActioonTask : PooledTask<Action, Action>
    {
        public PooledActioonTask(Action @do, Action callback) : base(@do, callback)
        {
        }
    }
    class StaticTaskPool<Signature> where Signature : ITaskPool
    {
        static Queue<object> erasedQueue;
        static void QueueAction(Action @do, Action callback)
        {

        }
    }
    class TaskPool<Signature> where Signature : ITaskPool
    {
        static Queue<object> erasedQueue;
    }
}
