using System;
using System.Collections.Generic;

namespace CatsDinner.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> execute)
        {
            foreach (var item in collection)
                execute(item);
        }
    }
}

