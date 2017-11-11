using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpPlayground.Library
{
    abstract class TaskPool
    {

        public void PutTask(Action task)
        {
            OnPutTask(task);
        }

        public void CancelTask(Action task)
        {
            OnCancelTask(task);
        }

        protected abstract void OnPutTask(Action task);
        protected abstract void OnCancelTask(Action task);
    }

    abstract class QueuedTaskPool : TaskPool
    {
        protected Queue<Action> queue = new Queue<Action>();

        protected override void OnCancelTask(Action task)
        {
            throw new TaskCanceledException();
        }

        protected override void OnPutTask(Action task)
        {
            queue.Enqueue(task);
        }


    }

    abstract class StackTaskPool : TaskPool
    {
        protected Stack<Action> queue = new Stack<Action>();

        protected override void OnCancelTask(Action task)
        {
            throw new TaskCanceledException();
        }

        protected override void OnPutTask(Action task)
        {
            queue.Push(task);
        }
    }

    class SingleThreadExecuteTaskPool : QueuedTaskPool
    {
        private bool locked = false;
        protected override void OnPutTask(Action task)
        {
            base.OnPutTask(task);
            runTask();
        }

        private void runTask()
        {
            if (locked) while (locked) { Thread.Sleep(1); };
            locked = !locked;
            if (locked)
            {
                while (queue.TryDequeue(out Action item))
                {
                    item();
                }
                locked = false;
            }
        }
    }

    class SingleThreadDelayTaskPool : QueuedTaskPool
    {
        Action Deliay;
        private bool locked;
        public SingleThreadDelayTaskPool(Action delay)
        {
            Deliay = delay;
        }

        protected override void OnPutTask(Action task)
        {
            queue.Enqueue(task);
            runTask();
        }

        private void runTask()
        {
            if (locked) while (locked) { Thread.Sleep(1); };
            locked = !locked;
            if (locked)
            {
                while (queue.TryDequeue(out Action item))
                {
                    item();
                    Deliay();
                }
                locked = false;
            }
        }
    }

    class ParallelTaskPool : QueuedTaskPool
    {
        public void Start()
        {
            Parallel.Invoke(queue.ToArray());
        }
    }
}
