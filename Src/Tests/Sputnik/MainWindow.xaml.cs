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

namespace IronJS.Tests.Sputnik
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BackgroundWorker worker = new BackgroundWorker();
        private string libPath;
        private volatile bool showExprTrees;

        private IList<TestGroup> testGroups;
        private TestGroup rootTestGroup;
        private HashSet<string> skipTests = new HashSet<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            this.skipTests.Add("S8.6_D1.2.js");
            this.skipTests.Add("S8.6_D1.3.js");
            this.skipTests.Add("S8.6_D1.4.js");
            this.skipTests.Add("S8.8_D1.3.js");
            this.skipTests.Add("S13.2_D1.2.js");

            this.worker.DoWork += this.RunTests;
            this.worker.ProgressChanged += this.Worker_ProgressChanged;
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;

            var rootPath = Path.Combine(new DirectoryInfo(GetExecutableDirectory()).Parent.Parent.FullName, "sputnik-v1");
            this.libPath = Path.Combine(rootPath, "lib");
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
                var group = new TestGroup(root, null)
                {
                    Name = Path.GetFileName(dir),
                };
                group.TestGroups = this.GenerateTestsList(group, basePath, dir);
                groups.Add(group);
            }

            foreach (var file in Directory.GetFiles(path, "*.js"))
            {
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

        private void LoadResults()
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load("tests.xml");
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
                    test.Status = info.Status;
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
            doc.Save("tests.xml");
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

            return gatherTests(this.rootTestGroup);
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            StartTests(GatherTests(g => g.Selected != false));
        }

        private void RunSingle_Click(object sender, RoutedEventArgs e)
        {
            var selected = this.FailedTests.SelectedItem as FailedTest;

            if (selected != null)
            {
                StartTests(new[] { selected.TestGroup });
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.StopButton.IsEnabled = false;
            this.worker.CancelAsync();
        }

        private void StartTests(IList<TestGroup> tests)
        {
            this.RunButton.IsEnabled = false;
            this.RunSingle.IsEnabled = false;
            this.StopButton.IsEnabled = true;
            this.FailedTests.Items.Clear();
            this.ExprTree.Text = string.Empty;
            this.ProgressBar.Foreground = new SolidColorBrush(Color.FromRgb(0x01, 0xD3, 0x28));
            SaveResults();
            this.worker.RunWorkerAsync(tests);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveResults();
            this.RunButton.IsEnabled = true;
            this.RunSingle.IsEnabled = true;
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

        private static IronJS.Hosting.Context CreateContext(Action<string> errorAction)
        {
            var ctx = IronJS.Hosting.Context.Create();
            var errorFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, errorAction);
            var failFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, new Action<string>(error => { throw new Exception(error); }));
            var printFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, new Action<string>((_) => { }));
            ctx.PutGlobal("$FAIL", failFunc);
            ctx.PutGlobal("ERROR", errorFunc);
            ctx.PutGlobal("$ERROR", errorFunc);
            ctx.PutGlobal("$PRINT", printFunc);
            return ctx;
        }

        private void AddFailed(TestGroup test, string error, bool regression)
        {
            var testCase = test.TestCase;

            Dispatcher.Invoke(new Action(() => this.FailedTests.Items.Add(new FailedTest
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

                bool pass = false;
                string resultingError = null;
                if (!this.skipTests.Contains(testCase.TestName))
                {
                    resultingError = RunTest(testCase);
                    pass = testCase.Negative ^ resultingError == null;
                }

                var previous = test.Status;

                if (pass)
                {
                    test.Status = Status.Passed;
                    passed++;
                    if (previous == Status.Failed)
                    {
                        improved++;
                    }
                }
                else
                {
                    bool regression = false;

                    test.Status = Status.Failed;
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

        private static string RunTest(TestCase testCase)
        {
            var errorText = new StringBuilder();
            var ctx = CreateContext(e => errorText.AppendLine(e));

            try
            {
                ctx.ExecuteFile(testCase.FullPath);
            }
            catch (Exception ex)
            {
                errorText.AppendLine("Exception: " + ex.GetBaseException().Message);
            }

            var error = errorText.ToString();
            return string.IsNullOrEmpty(error) ? null : error;
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            var selected = this.FailedTests.SelectedItem as FailedTest;

            if (selected != null)
            {
                LaunchFile(selected.Path);
            }
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

        private static TestGroup FindRoutedTestGroup(RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            var menu = menuItem.Parent as ContextMenu;
            var element = menu.PlacementTarget as FrameworkElement;
            return element.Tag as TestGroup;
        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            var testGroup = FindRoutedTestGroup(e);
            LaunchFile(testGroup.TestCase.FullPath);
        }
    }
}
