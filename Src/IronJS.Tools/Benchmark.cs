using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Scripting.Utils;

namespace IronJS.Utils
{
    /*
     * Credit for this class goes to 
     * the users Svish and Henk 
     * Holterman on Stack Overflow
     * */
    public sealed class Benchmark : IEnumerable<long>
    {
        private readonly Action subject;
        private Benchmark(Action subject) { this.subject = subject; }

        public static Benchmark This(Action subject)
        {
            return new Benchmark(subject);
        }

        public IEnumerator<long> GetEnumerator()
        {
            subject();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var watch = new Stopwatch();

            while (true)
            {
                watch.Reset();
                watch.Start();
                subject();
                watch.Stop();
                yield return watch.ElapsedMilliseconds;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
