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

        protected override TestResult ExecuteTest(IronJS.Hosting.CSharp.Context ctx, string test)
        {
            var totalTime = TimeSpan.Zero;
            var times = new List<long>();

            try
            {
                for (int i = 0; i < Runs; i++)
                {
                    var sw = Stopwatch.StartNew();
                    ctx.ExecuteFile(test);
                    sw.Stop();

                    totalTime += sw.Elapsed;
                    times.Add(sw.ElapsedMilliseconds);
                }
            }
            catch (Exception ex)
            {
                return new TestResult { Error = ex.GetBaseException().Message, Tag = Tuple.Create(test, Enumerable.Repeat(-1L, Runs).ToArray()) };
            }

            var average = (double)totalTime.TotalMilliseconds / Runs;
            return new TestResult { Score = average.ToString("0.##") + "ms", Tag = Tuple.Create(test, times.ToArray()) };
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

            return new TestResult { Score = "TODO: Calculate the total and 95% confidence interval for the suite." };
        }
    }
}
