using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Benchmarks
{
    public class SunSpiderTestSuite : TestSuite
    {
        private const int Runs = 10;

        public SunSpiderTestSuite(string basePath)
            : base(Path.Combine(basePath, "sunspider-0.9.1"))
        {
        }

        public override string SuiteName
        {
            get { return "SunSpider 0.9.1"; }
        }

        private static readonly double[] Distribution = { double.NaN, double.NaN, 12.71, 4.30, 3.18, 2.78, 2.57, 2.45, 2.36, 2.31, 2.26, 2.23, 2.20, 2.18, 2.16, 2.14, 2.13, 2.12, 2.11, 2.10, 2.09, 2.09, 2.08, 2.07, 2.07, 2.06, 2.06, 2.06, 2.05, 2.05, 2.05, 2.04, 2.04, 2.04, 2.03, 2.03, 2.03, 2.03, 2.03, 2.02, 2.02, 2.02, 2.02, 2.02, 2.02, 2.02, 2.01, 2.01, 2.01, 2.01, 2.01, 2.01, 2.01, 2.01, 2.01, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 2.00, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.99, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.98, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.97, 1.96 };

        private static double GetDistribution(int n)
        {
            if (n >= Distribution.Length)
            {
                n = Distribution.Length - 1;
            }

            return Distribution[n];
        }

        private static string GetScore(IList<long> times)
        {
            var runs = times.Count;

            var mean = times.Sum(t => (double)t) / runs;
            var stdDev = Math.Sqrt((from i in times
                                    let delta = i - mean
                                    let deltaSq = delta * delta
                                    select deltaSq).Sum() / (runs - 1));
            var stdErr = stdDev / Math.Sqrt(Runs);
            var confidence = ((GetDistribution(Runs) * stdErr / mean) * 100);
            return Math.Round(mean, 1).ToString("0.0") + "ms ± " + Math.Round(confidence, 1).ToString("0.0") + "%";
        }

        protected override TestResult ExecuteTest(IronJS.Hosting.CSharp.Context ctx, string test)
        {
            var times = new List<long>();

            try
            {
                for (int i = 0; i < Runs; i++)
                {
                    var sw = Stopwatch.StartNew();
                    ctx.ExecuteFile(test);
                    sw.Stop();

                    times.Add(sw.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                return new TestResult { Error = ex.GetBaseException().Message, Tag = Tuple.Create(test, Enumerable.Repeat(-1L, Runs).ToArray()) };
            }

            return new TestResult { Score = GetScore(times), Tag = Tuple.Create(test, times.ToArray()) };
        }

        protected override TestResult AggregateResults(IList<TestResult> results)
        {
            var times = from r in results
                        let tag = r.Tag as Tuple<string, long[]>
                        select new
                        {
                            Test = Path.GetFileNameWithoutExtension(tag.Item1),
                            Results = tag.Item2.Select(t => t.ToString()).ToArray()
                        };

            var scoresElements = from t in times
                                 select "\"" + t.Test + "\":[" + string.Join(",", t.Results) + "]";

            var scores = "{\"v\": \"sunspider-0.9.1\", " + string.Join(",", scoresElements) + "}";
            var scoresUrl = "http://www.webkit.org/perf/sunspider-0.9.1/sunspider-0.9.1/results.html?" +
                scores.Replace("\"", "%22").Replace(" ", "%20");

            using (Process.Start(scoresUrl)) { }

            if (results.Where(r => !string.IsNullOrEmpty(r.Error)).Any())
            {
                return new TestResult { Error = "Could not aggregate the score, because errors exist." };
            }

            var runs = new long[Runs];
            foreach (var result in results)
            {
                var t = (result.Tag as Tuple<string, long[]>).Item2;
                for (int i = 0; i < Runs; i++)
                {
                    runs[i] += t[i];
                }
            }

            return new TestResult { Score = GetScore(runs) };
        }
    }
}
