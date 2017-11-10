using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpPlayground
{

    delegate T1 BinderHelper<out T1>();
    delegate T2 BinderHelper<in T1, out T2>(T1 a);
    delegate T3 BinderHelper<in T1, in T2, out T3>(T1 a, T2 b);
    delegate T4 BinderHelper<in T1, in T2, in T3, out T4>(T1 a, T2 b, T3 c);
    delegate T5 BinderHelper<in T1, in T2, in T3, in T4, out T5>(T1 a, T2 b, T3 c, T4 d);

    static class BinderHelper
    {
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, Func<T7, Func<T8, Func<T9, TR>>>>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR> func) { return p1 => p2 => p3 => p4 => p5 => p6 => p7 => p8 => p9 => func(p1, p2, p3, p4, p5, p6, p7, p8, p9); }

        public static Binder<T1, T2, T3, T4, T5> ConvertFuncAsBinder<T1, T2, T3, T4, T5>(BinderHelper<T1, T2, T3, T4, T5> src) => src;
        public static Binder<T1, T2, T3, T4> ConvertFuncAsBinder<T1, T2, T3, T4>(BinderHelper<T1, T2, T3, T4> src) => src;
        public static Binder<T1, T2, T3> ConvertFuncAsBinder<T1, T2, T3>(BinderHelper<T1, T2, T3> src) => src;
        public static Binder<T1, T2> ConvertFuncAsBinder<T1, T2>(BinderHelper<T1, T2> src) => src;
        public static Binder<T1> ConvertFuncAsBinder<T1>(BinderHelper<T1> src) => src;


        public static BinderHelper<T2, T3, T4, T5> BindT1<T1, T2, T3, T4, T5>(this BinderHelper<T1, T2, T3, T4, T5> src, T1 a) => ConvertFuncAsBinder(src).BindT1(a);
        public static BinderHelper<T1, T3, T4, T5> BindT2<T1, T2, T3, T4, T5>(this BinderHelper<T1, T2, T3, T4, T5> src, T2 b) => ConvertFuncAsBinder(src).BindT2(b);
        public static BinderHelper<T1, T2, T4, T5> BindT3<T1, T2, T3, T4, T5>(this BinderHelper<T1, T2, T3, T4, T5> src, T3 c) => ConvertFuncAsBinder(src).BindT3(c);
        public static BinderHelper<T1, T2, T3, T5> BindT4<T1, T2, T3, T4, T5>(this BinderHelper<T1, T2, T3, T4, T5> src, T4 d) => ConvertFuncAsBinder(src).BindT4(d);


        public static BinderHelper<T2, T3, T4> BindT1<T1, T2, T3, T4>(this BinderHelper<T1, T2, T3, T4> src, T1 a) => ConvertFuncAsBinder(src).BindT1(a);
        public static BinderHelper<T1, T3, T4> BindT2<T1, T2, T3, T4>(this BinderHelper<T1, T2, T3, T4> src, T2 b) => ConvertFuncAsBinder(src).BindT2(b);
        public static BinderHelper<T1, T2, T4> BindT3<T1, T2, T3, T4>(this BinderHelper<T1, T2, T3, T4> src, T3 c) => ConvertFuncAsBinder(src).BindT3(c);

        public static BinderHelper<T2, T3> BindT1<T1, T2, T3>(this BinderHelper<T1, T2, T3> src, T1 a) => ConvertFuncAsBinder(src).BindT1(a);
        public static BinderHelper<T1, T3> BindT2<T1, T2, T3>(this BinderHelper<T1, T2, T3> src, T2 b) => ConvertFuncAsBinder(src).BindT2(b);

        public static BinderHelper<T2> BindT1<T1, T2>(this BinderHelper<T1, T2> src, T1 a) => ConvertFuncAsBinder(src).BindT1(a);
    }

    class Binder<T1>
    {
        BinderHelper<T1> outer;

        public static implicit operator Binder<T1>(BinderHelper<T1> src) => new Binder<T1>() { outer = src };

        public T1 Result() => outer();
    }

    class Binder<T1, T2>
    {
        BinderHelper<T1, T2> outer;

        public static implicit operator Binder<T1, T2>(BinderHelper<T1, T2> src) => new Binder<T1, T2>() { outer = src };

        public BinderHelper<T2> BindT1(T1 a) => () => outer(a);

        public T2 Result(T1 a) => outer(a); 
    }

    class Binder<T1, T2 ,T3>
    {
        BinderHelper<T1, T2, T3> outer;

        public static implicit operator Binder<T1, T2, T3>(BinderHelper<T1, T2, T3> src) => new Binder<T1, T2, T3>() { outer = src };

        public BinderHelper<T2, T3> BindT1(T1 a) => (T2 b) => outer(a, b);

        public BinderHelper<T1, T3> BindT2(T2 b) => (T1 a) => outer(a, b);

        public T3 Result(T1 a, T2 b) => outer(a, b);
    }

    class Binder<T1, T2, T3, T4>
    {
        BinderHelper<T1, T2, T3, T4> outer;

        public static implicit operator Binder<T1, T2, T3, T4>(BinderHelper<T1, T2, T3, T4> src) => new Binder<T1, T2, T3, T4>() { outer = src };

        public BinderHelper<T2, T3, T4> BindT1(T1 a) => (T2 b, T3 c) => outer(a, b, c);

        public BinderHelper<T1, T3, T4> BindT2(T2 b) => (T1 a, T3 c) => outer(a, b, c);

        public BinderHelper<T1, T2, T4> BindT3(T3 c) => (T1 a, T2 b) => outer(a, b, c);

        public BinderHelper<T1, T4> BindT2T3(T2 b, T3 c) => (T1 a) => outer(a, b, c);

        public T4 Result(T1 a, T2 b, T3 c) => outer(a, b, c);

    }

    class Binder<T1, T2, T3, T4, T5>
    {
        BinderHelper<T1, T2, T3, T4, T5> outer;

        public static implicit operator Binder<T1, T2, T3, T4, T5>(BinderHelper<T1, T2, T3, T4, T5> src) => new Binder<T1, T2, T3, T4, T5>() { outer = src };

        public BinderHelper<T2, T3, T4, T5> BindT1(T1 a) => (T2 b, T3 c, T4 d) => outer(a, b, c, d);
    
        public BinderHelper<T1, T3, T4, T5> BindT2(T2 b) => (T1 a, T3 c, T4 d) => outer(a, b, c, d);

        public BinderHelper<T1, T2, T4, T5> BindT3(T3 c) => (T1 a, T2 b, T4 d) => outer(a, b, c, d);

        public BinderHelper<T1, T2, T3, T5> BindT4(T4 d) => (T1 a, T2 b, T3 c) => outer(a, b, c, d);
        
        public BinderHelper<T1, T4, T5> BindT2T3(T2 b, T3 c) => (T1 a, T4 d) => outer(a, b, c, d);

        public BinderHelper<T1, T2, T5> BindT3T4(T3 c, T4 d) => (T1 a, T2 b) => outer(a, b, c, d);

        public T5 Result(T1 a, T2 b, T3 c, T4 d) => outer(a, b, c, d);
    }
}
