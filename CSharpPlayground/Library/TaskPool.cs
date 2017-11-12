using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpPlayground.Library
{
    /// <summary>
    /// The base TaskPool implement for Drivend Pool
    /// </summary>
    abstract class TaskPool
    {
        /// <summary>
        /// Put a task to current pool
        /// <para>Invoke this will cast <see cref="OnPutTask(Action)"/> method</para>
        /// </summary>
        /// <param name="task"></param>
        public void PutTask(Action task)
        {
            OnPutTask(task);
        }

        /// <summary>
        /// Request to cancel one task
        /// <para>Invoke this will cast <see cref="OnCancelTask(Action)"/> method</para>
        /// </summary>
        /// <param name="task"></param>
        public void CancelTask(Action task)
        {
            OnCancelTask(task);
        }

        /// <summary>
        /// Virtual <see cref="OnPutTask(Action)"/> method for subclasses override.
        /// </summary>
        /// <param name="task"></param>
        protected abstract void OnPutTask(Action task);
        /// <summary>
        /// Virtual <see cref="OnCancelTask(Action)"/> method for subclasses override.
        /// </summary>
        /// <param name="task"></param>
        protected abstract void OnCancelTask(Action task);
    }

    /// <summary>
    /// TaskPool manager task with <see cref="Queue{T}"/>
    /// </summary>
    abstract class QueuedTaskPool : TaskPool
    {
        protected Queue<Action> queue;

        public QueuedTaskPool()
        {
            queue = new Queue<Action>();
        }

        /// <summary>
        /// Queue task pool can't cancel task.
        /// </summary>
        /// <param name="task"></param>
        protected override void OnCancelTask(Action task)
        {
            throw new TaskCanceledException();
        }

        protected override void OnPutTask(Action task)
        {
            if (queue.Count == 0) queue.Enqueue(() => {});
            queue.Enqueue(task);
        }


    }

    /// <summary>
    /// TaskPool manager task with <see cref="Stack{T}"/>
    /// </summary>
    abstract class StackTaskPool : TaskPool
    {
        protected Stack<Action> queue;

        public StackTaskPool()
        {
            queue = new Stack<Action>();
        }

        protected override void OnCancelTask(Action task)
        {
            throw new TaskCanceledException();
        }

        protected override void OnPutTask(Action task)
        {
            if (queue.Count == 0) queue.Push(() => { });
            queue.Push(task);
        }
    }

    /// <summary>
    /// A base single thread FIFO(<see cref="Queue{T}"/>) task pool
    /// </summary>
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
                    item?.Invoke();
                }
                locked = false;
            }
        }
    }

    /// <summary>
    /// A base single thread FIFO(<see cref="Queue{T}"/>) task pool with a delay <see cref="Action"/>
    /// </summary>
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
                    item?.Invoke();
                    Deliay?.Invoke();
                }
                locked = false;
            }
        }
    }

    /// <summary>
    /// A parallel task pool
    /// </summary>
    class ParallelTaskPool : QueuedTaskPool
    {
        public void Start()
        {
            Parallel.Invoke(queue.ToArray());
        }
    }
}
