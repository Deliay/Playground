using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CSharpPlayground.Library
{
    static class Functional
    {
        public static B foldl<A, B>(Func<B, A, B> λ, B δ, IEnumerable<A> xs) => xs.Count() == 0 ? δ : foldl(λ, λ(δ, xs.First()), xs.Skip(1));

        public static B foldr<A, B>(Func<A, B, B> λ, B δ, IEnumerable<A> xs) => xs.Count() == 0 ? δ : λ(xs.First(), foldr(λ, δ, xs.Skip(1)));

        public static int LengthL<A>(this IEnumerable<A> xs) => foldl((x, a) => x + 1, 0, xs);

        public static int LengthR<A>(this IEnumerable<A> xs) => foldr((a, x) => x + 1, 0, xs);


    }
}
