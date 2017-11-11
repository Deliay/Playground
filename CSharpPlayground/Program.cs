using CSharpPlayground.Library;
using System;
using System.Globalization;
using System.Numerics;
using System.Threading;

namespace CSharpPlayground
{
    class Program
    {
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

        static void undoredo()
        {
            ActionBin history = new ActionBin();

            Integer a = 0;
            history.Do<Integer, int>(new PlusNumber(a, 5));
            history.Do<Integer, int>(new PlusNumber(a, 10));
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

            //在新线程中测试并行线程池
            Thread parallel = new Thread(() =>
            {
                
                int parallel_value = 0;
                for (int i = 0; i < 100; i++)
                {
                    test.PutTask(() => Console.Write($"{parallel_value++},"));
                }
                test.Start();
                Console.WriteLine();
            });

            //在新线程中测试延时线程池
            Thread delay = new Thread(() =>
            {
                int value = 0;
                for (int i = 0; i < 20; i++)
                {
                    singleThreadDelayPool.PutTask(() => Console.Write($"{value++},"));
                }
            });

            //在新线程中测试顺序线程池
            Thread index = new Thread(() =>
            {
                int value = 0;
                Console.WriteLine();
                for (int i = 0; i < 100; i++)
                {
                    singleThreadPool.PutTask(() => Console.Write($"{value++},"));
                }
            });

            //测试自旋锁排队
            Thread index2 = new Thread(() =>
            {
                int value = 100;
                Console.WriteLine();
                for (int i = 0; i < 100; i++)
                {
                    singleThreadPool.PutTask(() => Console.Write($"{value++},"));
                }
            });

            //测试自旋锁延迟排队
            Thread delay2 = new Thread(() =>
            {
                int value = 0;
                for (int i = 0; i < 20; i++)
                {
                    singleThreadDelayPool.PutTask(() => Console.Write($"{value++},"));
                }
            });

            index.Start();
            delay.Start();
            parallel.Start();
            delay2.Start();
            index2.Start();
        }

        static void Main(string[] args)
        {
            taskpool();

        }
    }
}
