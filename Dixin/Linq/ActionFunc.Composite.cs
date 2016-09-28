namespace Dixin.Linq.CSharp
{
    using System;

    public static partial class FuncExtensions
    {
        public static Func<T, TResult2> After<T, TResult1, TResult2>(
            this Func<TResult1, TResult2> function2, Func<T, TResult1> function1) =>
                value => function2(function1(value));

        public static Func<T, TResult2> Before<T, TResult1, TResult2>(
            this Func<T, TResult1> function1, Func<TResult1, TResult2> function2) =>
                value => function2(function1(value));
    }
}