using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml;
using System.Collections.ObjectModel;

namespace IronJS.Tests.Sputnik
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BackgroundWorker worker = new BackgroundWorker();
        private string libPath;
        private static readonly Dictionary<string, string> includeCache = new Dictionary<string, string>();
        private volatile bool showExprTrees;

        private IList<TestGroup> testGroups;
        private TestGroup rootTestGroup;
        private HashSet<string> ignoreTests = new HashSet<string>();
        private List<string> ignoreFixtures = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            this.FailedTests = new ObservableCollection<FailedTest>();

            InitializeComponent();

            this.ignoreTests.Add("S15.5.4.14_A1_T6");
            this.ignoreTests.Add("S15.5.4.14_A1_T7");
            this.ignoreTests.Add("S15.5.4.14_A1_T8");
            this.ignoreTests.Add("S15.5.4.14_A1_T9");
            this.ignoreTests.Add("S15.5.4.14_A2_T7");
            this.ignoreTests.Add("S15.5.4.8_A1_T11");
            this.ignoreTests.Add("S15.5.4.11_A3_T1");
            this.ignoreTests.Add("S15.5.4.11_A3_T2");
            this.ignoreTests.Add("S15.5.4.11_A3_T3");
            this.ignoreFixtures.Add("Unicode\\Unicode_218\\");

            this.worker.DoWork += this.RunTests;
            this.worker.ProgressChanged += this.Worker_ProgressChanged;
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;

            var rootPath = Path.Combine(new DirectoryInfo(GetExecutableDirectory()).Parent.Parent.FullName, "sputnik-v1");
            libPath = Path.Combine(rootPath, "lib");
            var testsPath = Path.Combine(rootPath, "tests");

            this.rootTestGroup = new TestGroup(null, null)
            {
                Name = "Sputnik v1"
            };
            rootTestGroup.TestGroups = GenerateTestsList(rootTestGroup, testsPath, testsPath);
            this.TestGroups = new[] { rootTestGroup };

            this.LoadResults();

            IronJS.Support.Debug.registerExprPrinter(this.ExprPrinter);
        }

        private IList<TestGroup> GenerateTestsList(TestGroup root, string basePath, string path)
        {
            var groups = new List<TestGroup>();

            foreach (var dir in Directory.GetDirectories(path))
            {
                if (this.ignoreFixtures.Any(i => (dir + "\\").StartsWith(Path.Combine(basePath, i))))
                {
                    continue;
                }

                var group = new TestGroup(root, null)
                {
                    Name = Path.GetFileName(dir),
                };
                group.TestGroups = this.GenerateTestsList(group, basePath, dir);
                groups.Add(group);
            }

            foreach (var file in Directory.GetFiles(path, "*.js"))
            {
                if (this.ignoreTests.Contains(Path.GetFileNameWithoutExtension(file)))
                {
                    continue;
                }

                var testCase = new TestCase(basePath, file);
                var group = new TestGroup(root, testCase)
                {
                    Name = testCase.TestName,
                };
                groups.Add(group);
            }

            return groups;
        }

        public IList<TestGroup> TestGroups
        {
            get
            {
                return this.testGroups;
            }

            private set
            {
                this.testGroups = value;
                var e = this.PropertyChanged;
                if (e != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("TestGroups"));
                }
            }
        }

        public ObservableCollection<FailedTest> FailedTests { get; set; }

        private void LoadResults()
        {
            var path = new DirectoryInfo(GetExecutableDirectory()).Parent.Parent.FullName;

            XDocument doc;
            try
            {
                doc = XDocument.Load(Path.Combine(path, "tests.xml"));
            }
            catch (IOException)
            {
                return;
            }
            catch (XmlException)
            {
                return;
            }

            var root = doc.Element("Tests");
            if (root == null)
            {
                return;
            }

            var tests = (from e in root.Elements("Test")
                         select new
                         {
                             Path = (string)e.Attribute("Path"),
                             Status = (Status)Enum.Parse(typeof(Status), (string)e.Attribute("Status"), true),
                             Selected = (bool)e.Attribute("Selected")
                         }).ToDictionary(e => e.Path);

            foreach (var test in this.GatherTests(g => true))
            {
                var key = test.TestCase.RelativePath;
                if (tests.ContainsKey(key))
                {
                    var info = tests[key];
                    test.Failed = info.Status == Status.Unknown
                        ? (int?)null
                        : (info.Status == Status.Failed ? 1 : 0);
                    test.Selected = info.Selected;
                }
            }
        }

        private void SaveResults()
        {
            var doc = new XElement("Tests",
                          from t in this.GatherTests(g => true)
                          select new XElement("Test",
                              new XAttribute("Path", t.TestCase.RelativePath),
                              new XAttribute("Status", t.Status.ToString()),
                              new XAttribute("Selected", t.Selected)));

            var path = new DirectoryInfo(GetExecutableDirectory()).Parent.Parent.FullName;
            doc.Save(Path.Combine(path, "tests.xml"));
        }

        private static string GetExecutableDirectory()
        {
            return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }

        void PrntExpr(string expr)
        {
            if (this.showExprTrees)
            {
                this.ExprTree.Text += expr;
            }
        }

        void ExprPrinter(string expr)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action<string>)this.PrntExpr, expr);
        }

        private IList<TestGroup> GatherTests(Func<TestGroup, bool> hierarchicalCriteria)
        {
            return this.GatherTests(this.rootTestGroup, hierarchicalCriteria);
        }

        private IList<TestGroup> GatherTests(TestGroup rootTestGroup, Func<TestGroup, bool> hierarchicalCriteria)
        {
            Func<TestGroup, IList<TestGroup>> gatherTests = null;
            gatherTests = rootGroup =>
            {
                if (rootGroup.TestCase != null)
                {
                    return new[] { rootGroup };
                }

                var groups = new List<TestGroup>();
                foreach (var group in rootGroup.TestGroups.Where(hierarchicalCriteria))
                {
                    groups.AddRange(gatherTests(group));
                }

                return groups;
            };

            return gatherTests(rootTestGroup);
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            StartTests(GatherTests(g => g.Selected != false));
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.StopButton.IsEnabled = false;
            this.worker.CancelAsync();
        }

        private void StartTests(IList<TestGroup> tests)
        {
            this.RunButton.IsEnabled = false;
            this.StopButton.IsEnabled = true;
            this.FailedTests.Clear();
            this.ExprTree.Text = string.Empty;
            this.ProgressBar.Foreground = new SolidColorBrush(Color.FromRgb(0x01, 0xD3, 0x28));
            SaveResults();
            this.worker.RunWorkerAsync(tests);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveResults();
            this.RunButton.IsEnabled = true;
            this.StopButton.IsEnabled = false;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var state = e.UserState as Tuple<int, int, int, int, int>;
            var total = state.Item1;
            var passed = state.Item2;
            var failed = state.Item3;
            var improved = state.Item4;
            var regressed = state.Item5;

            if (failed > 0)
            {
                this.ProgressBar.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x52));
            }

            this.ProgressBar.Maximum = total;
            this.ProgressBar.Value = (passed + failed);
            this.ProgressText.Content = (passed + failed) + "/" + total;

            this.PassedLabel.Content = "Passed: " + passed + " (" + improved + " improved)";
            this.FailedLabel.Content = "Failed: " + failed + " (" + regressed + " regressed)";
        }

        private static void LaunchFile(string path)
        {
            var info = new ProcessStartInfo("C:\\Windows\\notepad.exe", "\"" + path + "\"");
            Process.Start(info);
        }

        private static string GetInclude(string libPath, string file)
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

        private static IronJS.Hosting.CSharp.Context CreateContext(string libPath, Action<string> errorAction)
        {
            var ctx = new IronJS.Hosting.CSharp.Context();

            Action<string> failAction = error => { throw new Exception(error); };
            Action<string> printAction = message => Debug.WriteLine(message);
            Action<string> includeAction = file => ctx.Execute(GetInclude(libPath, file));

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

        private void AddFailed(TestGroup test, string error, bool regression)
        {
            var testCase = test.TestCase;

            Dispatcher.Invoke(new Action(() => this.FailedTests.Add(new FailedTest
            {
                Name = testCase.TestName,
                Path = testCase.FullPath,
                Assertion = testCase.Assertion,
                Exception = error,
                TestGroup = test,
                Regression = regression,
            })));
        }

        void UpdateCurrentTest(TestCase test)
        {
            Dispatcher.Invoke(new Action(() => this.CurrentTest.Content = (test == null ? string.Empty : test.TestName)));
        }

        private void RunTests(object sender, DoWorkEventArgs args)
        {
            var tests = args.Argument as IList<TestGroup>;
            var testCount = tests.Count;

            int passed = 0;
            int failed = 0;
            int improved = 0;
            int regressed = 0;

            this.worker.ReportProgress(0, Tuple.Create(testCount, passed, failed, improved, regressed));

            for (int i = 0; i < testCount; i++)
            {
                var test = tests[i];
                var testCase = test.TestCase;

                if (this.worker.CancellationPending)
                {
                    break;
                }

                this.UpdateCurrentTest(testCase);

                var resultingError = RunTest(this.libPath, testCase);
                var pass = testCase.Negative ^ resultingError == null;

                var previous = test.Status;

                if (pass)
                {
                    test.Failed = 0;
                    passed++;
                    if (previous == Status.Failed)
                    {
                        improved++;
                    }
                }
                else
                {
                    bool regression = false;

                    test.Failed = 1;
                    failed++;
                    if (previous == Status.Passed)
                    {
                        regressed++;
                        regression = true;
                    }

                    this.AddFailed(test, testCase.Negative ? "Expected Exception" : (resultingError ?? "<missing error message>"), regression);
                }

                this.worker.ReportProgress((int)Math.Round(99.0 * i / testCount), Tuple.Create(testCount, passed, failed, improved, regressed));
            }

            this.UpdateCurrentTest(null);
            this.worker.ReportProgress(100, Tuple.Create(testCount, passed, failed, improved, regressed));
        }

        private static string RunTest(string libPath, TestCase testCase)
        {
            var errorText = new StringBuilder();
            var ctx = CreateContext(libPath, e => errorText.AppendLine(e));

            try
            {
                ctx.ExecuteFile(testCase.FullPath);
            }
            catch (Exception ex)
            {
                errorText.AppendLine("Exception: " + ex.ToString());
            }

            var error = errorText.ToString();
            return string.IsNullOrEmpty(error) ? null : error;
        }

        private void ShowExprTrees_Checked(object sender, RoutedEventArgs e)
        {
            var isChecked = ((CheckBox)sender).IsChecked;

            this.showExprTrees = isChecked ?? false;

            this.ExpressionTreeRow.Height = isChecked ?? false
                ? new GridLength(1, GridUnitType.Star)
                : new GridLength(0, GridUnitType.Pixel);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveResults();
        }

        private static object FindRoutedTestGroup(RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var menu = menuItem.Parent as ContextMenu;
            var element = menu.PlacementTarget as FrameworkElement;
            return element.Tag;
        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            var test = FindRoutedTestGroup(e);

            var testGroup = test as TestGroup;
            if (testGroup != null)
            {
                LaunchFile(testGroup.TestCase.FullPath);
            }

            var failedTest = test as FailedTest;
            if (failedTest != null)
            {
                LaunchFile(failedTest.Path);
            }
        }

        private void PerformSelection(RoutedEventArgs e, bool select, Predicate<TestGroup> filter)
        {
            var testGroup = FindRoutedTestGroup(e) as TestGroup;
            foreach (var test in GatherTests(testGroup, tg => true))
            {
                if (filter(test))
                {
                    test.Selected = select;
                }
            }
        }

        private void DeselectUnknown_Click(object sender, RoutedEventArgs e)
        {
            PerformSelection(e, false, tg => tg.Status == Status.Unknown);
        }

        private void DeselectPassed_Click(object sender, RoutedEventArgs e)
        {
            PerformSelection(e, false, tg => tg.Status == Status.Passed);
        }

        private void DeselectFailed_Click(object sender, RoutedEventArgs e)
        {
            PerformSelection(e, false, tg => tg.Status == Status.Failed);
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            PerformSelection(e, false, tg => true);
        }

        private void SelectUnknown_Click(object sender, RoutedEventArgs e)
        {
            PerformSelection(e, true, tg => tg.Status == Status.Unknown);
        }

        private void SelectPassed_Click(object sender, RoutedEventArgs e)
        {
            PerformSelection(e, true, tg => tg.Status == Status.Passed);
        }

        private void SelectFailed_Click(object sender, RoutedEventArgs e)
        {
            PerformSelection(e, true, tg => tg.Status == Status.Failed);
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            PerformSelection(e, true, tg => true);
        }
    }
}
