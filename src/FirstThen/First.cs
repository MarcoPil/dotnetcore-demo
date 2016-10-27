﻿using System;
using System.Collections.Generic;
using System.Linq;


namespace FirstThen
{
    public static class First
    {
        public static IInvoke<T, TResult> Do<T, TResult>(Func<T, TResult> a)
        {
            return new Invoker<T, TResult>(a);
        }

        public static IInvoke<T, T> Do<T>(Action<T> a)
        {
            return new Invoker<T, T>(m => m).Then(a);
        }

        public static IInvoke<T, T> Do<T>(Action a)
        {
            return new Invoker<T, T>(m => m).Then(a);
        }
    }
}
