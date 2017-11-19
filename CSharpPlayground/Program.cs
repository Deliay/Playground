using CSharpPlayground.Library;
using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace CSharpPlayground
{
    class Program
    {
        /// <summary>
        /// Bind functor and get high order function
        /// </summary>
        static void bind()
        {
            //explicit create bind
            BinderHelper<int, int, int> directAplusB = ((int a, int b) => a + b);

            //implicit create bind with Helper func
            var AopB = BinderHelper.ConvertFuncAsBinder((int a, Func<int, int, int> op, int c) => op(a, c));

            BinderHelper<Func<int, int>, Func<int, int>, BinderHelper<int, Func<int, int, int>, int, int>>
                bind = (a, b) => (a1, op, b1) => op(a(a1), b(b1));

            var func = bind.BindT1(a => a + 1).BindT1(a => a + 2)();

            var subFunc = func.BindT2((a, b) => a - b);
            var plusFunc = func.BindT2((a, b) => a + b);
            var mulFunc = func.BindT2((a, b) => a * b);
            // (a + 1) op (b + 2)
            // (5 + 1) - (2 + 2) == 6 - 4 == 2
            Console.WriteLine(subFunc(5, 2));
            // 1 + 1 + 2 + 2 == 6
            Console.WriteLine(plusFunc(1, 2));
            // (3 + 1) * (4 + 2) == 4 * 6 == 24
            Console.WriteLine(mulFunc(3, 4));
        }

        class Integer : IUndoable
        {
            public int value;
            public static implicit operator int(Integer n)
            {
                return n.value;
            }

            public static implicit operator Integer(int n)
            {
                return new Integer { value = n };
            }
        }

        class PlusNumber : UndoableAction<Integer, int>
        {
            public PlusNumber(Integer src, int arg) : base(src, arg)
            {
            }

            protected override void Apply(Integer src, int arg)
            {
                src.value += arg;
            }

            protected override void Restore(Integer src, int arg)
            {
                src.value -= arg;
            }
        }

        /// <summary>
        /// stack and queued undo/redo pool
        /// </summary>
        static void undoredo()
        {
            ActionBin history = new ActionBin();

            Integer a = 0;
            Console.WriteLine(a);
            history.Do<Integer, int>(new PlusNumber(a, 5));
            Console.WriteLine(a);
            history.Do<Integer, int>(new PlusNumber(a, 10));
            Console.WriteLine(a);
            history.Do<Integer, int>(new PlusNumber(a, 2));
            Console.WriteLine(a);

            history.Undo();
            Console.WriteLine(a);
            history.Redo();
            Console.WriteLine(a);
            history.Undo();
            Console.WriteLine(a);
            history.Undo();
            Console.WriteLine(a);
            history.Undo();
            Console.WriteLine(a);
        }

        /// <summary>
        /// Task pool, task in pool will in order to run 
        /// </summary>
        static void taskpool()
        {
            //共用的Task池
            SingleThreadDelayTaskPool singleThreadDelayPool = new SingleThreadDelayTaskPool(() =>
            {
                DateTime now = DateTime.Now;
                DateTime target = now.AddSeconds(0.15);
                while (DateTime.Now < target) { Thread.Sleep(1); }
            });
            ParallelTaskPool test = new ParallelTaskPool();
            SingleThreadExecuteTaskPool singleThreadPool = new SingleThreadExecuteTaskPool();

            //在新线程中测试并行任务池
            Thread parallel = new Thread(() =>
            {
                
                int parallel_value = 0;
                for (int i = 0; i < 100; i++)
                {
                    test.PutTask(() => Console.Write($"P-{parallel_value++},"));
                }
                test.Start();
                Console.WriteLine("\n=================================END PARALLEL");
            });

            //在新线程中测试延时任务池
            Thread delay = new Thread(() =>
            {
                int delay_value = 0;
                for (int i = 0; i < 50; i++)
                {
                    singleThreadDelayPool.PutTask(() => Console.Write($"D-{delay_value++},"));
                }
                Console.WriteLine("\n=================================END DELAY1");
            });

            //在新线程中测试顺序任务池
            Thread index = new Thread(() =>
            {
                int index_value = 0;
                for (int i = 0; i < 100; i++)
                {
                    singleThreadPool.PutTask(() => Console.Write($"I-{index_value++},"));
                }
                Console.WriteLine("\n=================================END INDEX1");
            });

            //测试自旋锁排队
            Thread index2 = new Thread(() =>
            {
                int index_value2 = 100;
                for (int i = 0; i < 100; i++)
                {
                    singleThreadPool.PutTask(() => Console.Write($"I2-{index_value2++},"));
                }
                Console.WriteLine("\n=================================END INDEX2");
            });

            //测试自旋锁延迟排队
            Thread delay2 = new Thread(() =>
            {
                int delay_value2 = 0;
                for (int j = 0; j < 50; j++)
                {
                    singleThreadDelayPool.PutTask(() => Console.Write($"D-{delay_value2++},"));
                }
                //排队完成后，再执行并行任务池
                Console.WriteLine("\n=================================END DELAY2");
                parallel.Start();
            });

            //并不确定先执行哪个，但是会排队

            index2.Start();
            index.Start();
            delay.Start();
            delay2.Start();
        }

        class 去幼儿园 : IBaseEvent
        {
            public DateTime 发车时间 = DateTime.Now;
        }

        class 上车 : BaseEventDispatcher<去幼儿园>
        {
            public DateTime 始发时间 = DateTime.Now;
        }

        /// <summary>
        /// Install a event dispathcer to a class extend <see cref="BaseEventDispatcher{T}"/>
        /// </summary>
        static void eventbus()
        {
            EventDispatcher.Instance.RegisterNewDispatcher<上车, 去幼儿园>();
            上车 早班车 = new 上车();
            早班车.BindEvent<去幼儿园>((进站) => { Console.WriteLine(进站.发车时间); });

            Console.WriteLine(早班车.始发时间);
            Thread.Sleep(3000);
            早班车.RaiseEvent(new 去幼儿园());

        }

        static void Main(string[] args)
        {
            eventbus();

        }
    }
}
