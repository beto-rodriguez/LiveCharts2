// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

namespace LiveChartsCore.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public static class ThreadingExtensions
    {
        private static readonly object s_globalLocker = new();

        /// <summary>
        /// Returns a locked an <see cref="IEnumerable{T}"/> instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="locker"></param>
        /// <returns></returns>
        public static IEnumerable<T> Lock<T>(this IEnumerable<T> enumerable, object? locker = null)
        {
            if (locker == null)
            {
                locker = enumerable is not ICollection collection
                    // notice if you pass an iterator it will be locked using this static variable.
                    // it might cause deadlocks
                    ? s_globalLocker
                    : collection.SyncRoot;
            }

            //lock (locker)
            //{
            var enumerator = enumerable.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }

            enumerator.Dispose();
            //}
        }
    }
}
