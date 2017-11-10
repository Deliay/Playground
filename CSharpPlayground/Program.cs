using System;
using System.Globalization;
using System.Numerics;

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

        static void Main(string[] args)
        {


        }
    }
}
