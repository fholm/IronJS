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

namespace IronJS.Tests.Sputnik
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BackgroundWorker worker = new BackgroundWorker();
        private string _libPath;
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
            this._libPath = Path.Combine(rootPath, "lib");
            var testsPath = Path.Combine(rootPath, "tests");

            this.rootTestGroup = new TestGroup(null, null)
            {
                Name = "Sputnik v1"
            };
            rootTestGroup.TestGroups = GenerateTestsList(rootTestGroup, testsPath, testsPath);
            this.TestGroups = new[] { rootTestGroup };

            IronJS.Support.Debug.registerExprPrinter(ExprPrinter);
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
                group.TestGroups = GenerateTestsList(group, basePath, dir);
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
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(PrntExpr), expr);
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            if (!this.worker.IsBusy)
            {
                Func<TestGroup, IList<TestGroup>> gatherTests = null;
                gatherTests = rootGroup =>
                {
                    if (rootGroup.TestCase != null)
                    {
                        return new[] { rootGroup };
                    }

                    var groups = new List<TestGroup>();
                    foreach (var group in rootGroup.TestGroups.Where(g => g.Selected != false))
                    {
                        groups.AddRange(gatherTests(group));
                    }

                    return groups;
                };

                StartTests(gatherTests(this.rootTestGroup));
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
            this.progressBar.Foreground = new SolidColorBrush(Color.FromRgb(0x01, 0xD3, 0x28));
            this.worker.RunWorkerAsync(tests);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.RunButton.IsEnabled = true;
            this.RunSingle.IsEnabled = true;
            this.StopButton.IsEnabled = false;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var state = e.UserState as Tuple<int, int, int>;
            var total = state.Item1;
            var passed = state.Item2;
            var failed = state.Item3;

            if (failed > 0)
            {
                progressBar.Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0x00, 0x52));
            }

            progressBar.Maximum = total;
            progressBar.Value = (passed + failed);
            progressText.Content = (passed + failed) + "/" + total;

            passedLabel.Content = "Passed: " + passed; // +" (" + prevPassed + ")";
            failedLabel.Content = "Failed: " + failed; // +" (" + prevFailed + ")";
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

        private void AddFailed(TestGroup test, string error)
        {
            var testCase = test.TestCase;

            Dispatcher.Invoke(new Action(() => this.FailedTests.Items.Add(new FailedTest
            {
                Name = testCase.TestName,
                Path = testCase.FullPath,
                Assertion = testCase.Assertion,
                Exception = error,
                TestGroup = test,
            })));
        }

        void UpdateCurrentTest(TestCase test)
        {
            Dispatcher.Invoke(new Action(() => currentTest.Text = test == null ? string.Empty : test.TestName));
        }

        private void RunTests(object sender, DoWorkEventArgs args)
        {
            var tests = args.Argument as IList<TestGroup>;
            var testCount = tests.Count;

            int passed = 0;
            int failed = 0;

            this.worker.ReportProgress(0, Tuple.Create(testCount, passed, failed));

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

                // TODO: Check for regression.

                if (pass)
                {
                    test.Status = Status.Passed;
                    passed++;
                }
                else
                {
                    test.Status = Status.Failed;
                    failed++;
                    AddFailed(test, testCase.Negative ? "Expected Exception" : (resultingError ?? "<missing error message>"));
                }

                this.worker.ReportProgress((int)Math.Round(99.0 * i / testCount), Tuple.Create(testCount, passed, failed));
            }

            this.UpdateCurrentTest(null);
            this.worker.ReportProgress(100, Tuple.Create(testCount, passed, failed));
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

        private void RunSingle_Click(object sender, RoutedEventArgs e)
        {
            var selected = this.FailedTests.SelectedItem as FailedTest;

            if (selected != null)
            {
                StartTests(new[] { selected.TestGroup });
            }
        }

        private void ShowExprTrees_Checked(object sender, RoutedEventArgs e)
        {
            this.showExprTrees = ((CheckBox)sender).IsChecked ?? false;
        }
    }
}
