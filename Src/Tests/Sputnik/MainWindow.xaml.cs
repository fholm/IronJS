using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

namespace IronJS.Tests.Sputnik {

  public partial class MainWindow : Window {
    TreeViewItem _testsRoot;

    string _rootPath;
    string _testsPath;
    string _libPath;
    string _selectedPath;

    HashSet<string> _skiptests =
      new HashSet<string>();

    int prevPassed;
    int prevFailed;

    List<string> _testFiles = new List<string>();

    public MainWindow() {
      InitializeComponent();

      _rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\sputnik-v1";
      _testsPath = _rootPath + @"\tests";
      _libPath = _rootPath + @"\lib";

      _testsRoot = new TreeViewItem();
      _testsRoot.Header = "Sputnik v1";

      this.Closing += MainWindow_Closing;

      _addSubTestGroups(_testsRoot, _testsPath);

      Tests.SelectedItemChanged += _testsSelectionChanged;
      Tests.Items.Add(_testsRoot);

      WindowState = System.Windows.WindowState.Maximized;

      failedTests.SelectionChanged += failedTests_SelectionChanged;

      if (File.Exists("last-test.log")) {
        selectedTestPath.Text = _selectedPath = File.ReadAllText("last-test.log");
      }

      if (File.Exists("prev-result.log")) {
        var x = File.ReadAllText("prev-result.log").Split(',');
        prevPassed = Int32.Parse(x[0]);
        prevFailed = Int32.Parse(x[1]);
      }

      IronJS.Support.Debug.registerExprPrinter(ExprPrinter);

      _skiptests.Add("S8.6_D1.2.js");
      _skiptests.Add("S8.6_D1.3.js");
      _skiptests.Add("S8.6_D1.4.js");
      _skiptests.Add("S8.8_D1.3.js");
      _skiptests.Add("S13.2_D1.2.js");
    }

    void prntExpr(string expr) {
      if (showExprTreesChk.IsChecked ?? false) {
        exprTree.Text += expr;
      }
    }

    void ExprPrinter(string expr) {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(prntExpr), expr);
    }

    Thread thread;

    void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      _stop = true;
      if (thread != null) {
        thread.Abort();
      }
    }

    string _openPath;
    void failedTests_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      var test = (sender as ListView).SelectedItem as FailedTest;
      if (test != null) {
        _openPath = test.Path;
      }
    }

    void _testsSelectionChanged(object s, RoutedPropertyChangedEventArgs<object> e) {
      selectedTestPath.Text = _selectedPath = _getPath(((TreeView)s).SelectedItem as TreeViewItem);
      try {
        File.WriteAllText("last-test.log", _selectedPath);

      } catch {

      }
    }

    void _collectTestFiles(string path) {
      foreach (var file in Directory.GetFiles(path)) {
        if (file.EndsWith(".js")) {
          _testFiles.Add(file);
        }
      }

      foreach (var dir in Directory.GetDirectories(path)) {
        _collectTestFiles(dir);
      }
    }

    void _updateProgress(int num, int passed, int failed) {
      progressText.Content = num + "/" + _testFiles.Count;
      progressBar.Value = num;
      progressBar.Maximum = _testFiles.Count;

      passedLabel.Content = "Passed: " + passed + " (" + prevPassed + ")";
      failedLabel.Content = "Failed: " + failed + " (" + prevFailed + ")";
    }

    bool _debug;
    void run_Click(object sender, RoutedEventArgs e) {
      if (_stop) {
        _testFiles.Clear();
        _collectTestFiles(_testsPath + "\\" + _selectedPath);
        doRunTests();
      }
    }

    void doRunTests() {
      if (_stop) {
        failedTests.Items.Clear();
        _updateProgress(0, 0, 0);
        _debug = debugBox.IsChecked ?? false;
        ThreadStart testThread = runTests;
        thread = new Thread(testThread);
        thread.Start();
      
      }
    }

    void _addFailedTest(FailedTest test) {
      failedTests.Items.Add(test);
    }

    void openFile() {
      var info = new ProcessStartInfo("C:\\Windows\\notepad.exe", _openPath);
      Process.Start(info);
    }

    void Error(string error) {
      throw new Exception(error);
    }

    IronJS.Hosting.Context _createContext() {
      var ctx = IronJS.Hosting.Context.Create();
      var error = new Action<string>(Error);
      var errorFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, error);
      var printFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, new Action<string>((_) => { }));
      ctx.PutGlobal("$FAIL", errorFunc);
      ctx.PutGlobal("ERROR", errorFunc);
      ctx.PutGlobal("$ERROR", errorFunc);
      ctx.PutGlobal("$PRINT", printFunc);
      return ctx;
    }

    string _getAssertion(string testData) {
      var match = Regex.Match(testData, @"@assertion:(.*)");
      if (match.Success) {
        return match.Groups[1].Value.Trim();

      } else {
        return "<missing assertion>";
      }
    }

    string _exTrace(Exception ex) {
      if (ex == null) {
        return "";
      }

      return "\r\n" + ex.Message + ": \r\n" + ex.StackTrace + (_exTrace(ex.InnerException));
    }

    void _failed(string data, string path, string ex) {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action<FailedTest>(_addFailedTest), new FailedTest {
        Name = _getLastPathName(path),
        Path = path,
        Assertion = _getAssertion(data),
        Exception = ex ?? "<missing message>"
      });
    }

    void _updateProgress_Thread(int testsRun, int passed, int failed) {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action<int, int, int>(_updateProgress), testsRun, passed, failed);
    }

    void _updateCurrentTest(string test) {
      currentTest.Text = _getLastPathName(test);
      _openPath = test;
    }

    void _updateCurrentTest_Thread(string test) {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action<string>(_updateCurrentTest), test);
    }

    void ClearExpr() {
      exprTree.Text = "";
    }

    bool _stop = true;
    void runTests() {
      _stop = false;

      int testsRun = 0;
      int passed = 0;
      int failed = 0;

      foreach (var file in _testFiles) {
        if (_stop)
          break;

        if (file == null)
          continue;

        if (_skiptests.Contains(_getLastPathName(file))) {
          ++failed;
          _updateProgress_Thread(++testsRun, passed, failed);
          continue;
        }

        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(ClearExpr));

        var ctx = _createContext();
        var test = File.ReadAllText(file);
        var shouldFail = test.Contains("@negative");

        _updateCurrentTest_Thread(file);

        if (shouldFail) {
          try {
            ctx.Execute(test);
            ++failed;
            _failed(test, file, "Should Fail");
          } catch (Exception) {
            ++passed;
          }

        } else {
          if (_debug) {
            ctx.Execute(test);
            ++passed;

          } else {
            try {
              ctx.Execute(test);
              ++passed;
            } catch(Exception ex) {
              ++failed;
              _failed(test, file, _exTrace(ex).Trim());
            }
          }
        }

        _updateProgress_Thread(++testsRun, passed, failed);
      }

      _stop = true;
      prevPassed = passed;
      prevFailed = failed;

      File.WriteAllText("prev-result.log", prevPassed + "," + prevFailed);
    }

    string _getLastPathName(string path) {
      return path.Substring(path.LastIndexOf('\\') + 1);
    }

    string _getPath(TreeViewItem item) {
      var path = "";

      while (item != _testsRoot) {
        path = "\\" + (item.Header as string) + path;

        if (item.Parent is TreeView) {
          break;
        }

        item = (TreeViewItem)item.Parent;
      }

      return path.Trim('\\');
    }

    void _addSubTestGroups(TreeViewItem root, string path) {
      foreach (var dir in Directory.GetDirectories(path)) {
        var item = new TreeViewItem();
        item.Header = _getLastPathName(dir);
        _addSubTestGroups(item, dir);
        root.Items.Add(item);
      }
    }

    private void button2_Click(object sender, RoutedEventArgs e) {
      openFile();
    }

    private void button3_Click(object sender, RoutedEventArgs e) {
      _testFiles.Clear();
      _testFiles.Add(_openPath);
      doRunTests();
    }

  }

  public class FailedTest {
    public string Name { get; set; }
    public string Path { get; set; }
    public string Error { get; set; }
    public string Assertion { get; set; }
    public string Exception { get; set; }
  }
}
