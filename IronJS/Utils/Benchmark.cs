using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

namespace IronJS.Utils
{
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
