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

        public static A Compimum<A>(this IEnumerable<A> xs, Func<A, A, A> compare) => xs.Count() == 0 ? default(A) : foldr(compare, xs.First(), xs.Skip(1));

        public static IEnumerable<B> map<A, B>(Func<A, B> λ, IEnumerable<A> xs) => xs.Select(λ);

        public static IEnumerable<A> filter<A>(Func<A, bool> λ, IEnumerable<A> xs) => xs.Where(λ);

        public static IEnumerable<B> scanl<A, B>(Func<B, A, B> λ, B δ, IEnumerable<A> xs) => Enumerable.Append(default(IEnumerable<B>), δ).Union(scanl(λ, λ(δ, xs.First()), xs.Skip(1)));

        public static IEnumerable<B> scanr<A, B>(Func<A, B, B> λ, B δ, IEnumerable<A> xs)
        {
            if (xs.Count() == 0) return Enumerable.Append(default(IEnumerable<B>), δ);
            var ys = scanr(λ, δ, xs);
            return Enumerable.Append(default(IEnumerable<B>), λ(xs.First(), ys.First())).Union(ys);
        }
    }
}
