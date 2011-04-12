namespace IronJS.Tests.UnitTests.Sputnik
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    public class SputnikTestFixture
    {
        private static readonly string basePath;
        private static readonly string libPath;
        private static readonly Dictionary<string, string> includeCache = new Dictionary<string, string>();
        private readonly string testsPath;

        static SputnikTestFixture()
        {
            var assemblyPath = Program.GetAssemblyPath();
            var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(assemblyPath));

            basePath = assemblyDirectory.Parent.Parent.Parent.FullName;
            libPath = Path.Combine(basePath, "Sputnik", "sputnik-v1", "lib");
            basePath = Path.Combine(basePath, "Sputnik", "sputnik-v1", "tests");
        }

        public SputnikTestFixture(string testsPath)
        {
            this.testsPath = Path.Combine(basePath, testsPath);
        }

        private static string GetInclude(string file)
        {
            string source;

            if (!includeCache.TryGetValue(file, out source))
            {
                source = GetSpecialInclude(file);
                if (source == null)
                {
                    source = File.ReadAllText(Path.Combine(libPath, file));
                }

                includeCache.Add(file, source);
            }

            return source;
        }

        private static string GetSpecialInclude(string file)
        {
            if (file == "environment.js")
            {
                return GetTimeZoneInfoInclude();
            }

            return null;
        }

        private static string GetTimeZoneInfoInclude()
        {
            var local = TimeZoneInfo.Local;
            var now = DateTime.UtcNow;

            var daylightRule = local.GetAdjustmentRules().Where(a => a.DateEnd > now && a.DateStart <= now).Single();

            var start = daylightRule.DaylightTransitionStart;
            var end = daylightRule.DaylightTransitionEnd;

            var info = new StringBuilder();
            info.AppendLine(string.Format("var $DST_end_hour = {0};", end.TimeOfDay.Hour));
            info.AppendLine(string.Format("var $DST_end_minutes = {0};", end.TimeOfDay.Minute));
            info.AppendLine(string.Format("var $DST_end_month = {0};", end.Month));
            info.AppendLine(string.Format("var $DST_end_sunday = '{0}';", end.Day > 15 ? "last" : "first"));
            info.AppendLine(string.Format("var $DST_start_hour = {0};", start.TimeOfDay.Hour));
            info.AppendLine(string.Format("var $DST_start_minutes = {0};", start.TimeOfDay.Minute));
            info.AppendLine(string.Format("var $DST_start_month = {0};", start.Month));
            info.AppendLine(string.Format("var $DST_start_sunday = '{0}';", start.Day > 15 ? "last" : "first"));
            info.AppendLine(string.Format("var $LocalTZ = {0};", local.BaseUtcOffset.TotalSeconds / 3600));

            return info.ToString();
        }

        private static IronJS.Hosting.CSharp.Context CreateContext(Action<string> errorAction)
        {
            var ctx = new IronJS.Hosting.CSharp.Context();

            Action<string> failAction = error => Assert.Fail(error);
            Action<string> printAction = message => Trace.WriteLine(message);
            Action<string> includeAction = file => ctx.Execute(GetInclude(file));

            var errorFunc = IronJS.Native.Utils.CreateFunction(ctx.Environment, 1, errorAction);
            var failFunc = IronJS.Native.Utils.CreateFunction(ctx.Environment, 1, failAction);
            var printFunc = IronJS.Native.Utils.CreateFunction(ctx.Environment, 1, printAction);
            var includeFunc = IronJS.Native.Utils.CreateFunction(ctx.Environment, 1, includeAction);

            ctx.SetGlobal("$FAIL", failFunc);
            ctx.SetGlobal("ERROR", errorFunc);
            ctx.SetGlobal("$ERROR", errorFunc);
            ctx.SetGlobal("$PRINT", printFunc);
            ctx.SetGlobal("$INCLUDE", includeFunc);

            return ctx;
        }

        public void RunFile(string fileName)
        {
            StringBuilder errorText = new StringBuilder();

            try
            {
                fileName = Path.Combine(this.testsPath, fileName);
                var ctx = CreateContext(e => errorText.AppendLine(e));
                ctx.ExecuteFile(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Test threw an exception.", ex);
            }

            if (errorText.Length > 0)
            {
                Assert.Fail(errorText.ToString());
            }
        }
    }
}