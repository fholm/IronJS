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

namespace IronJS.Tests.Sputnik {
  public class FailedTest {
    public string Name { get; set; }
    public string Path { get; set; }
    public string Error { get; set; }
    public string Assertion { get; set; }
    public string Exception { get; set; }
  }

  public partial class MainWindow : Window {
    TreeViewItem _testsRoot;

    string _rootPath;
    string _testsPath;
    string _libPath;
    string _selectedPath;

    List<string> _testFiles = new List<string>();

    public MainWindow() {
      InitializeComponent();

      _rootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\sputnik-v1";
      _testsPath = _rootPath + @"\tests";
      _libPath = _rootPath + @"\lib";

      _testsRoot = new TreeViewItem();
      _testsRoot.Header = "Sputnik v1";

      _addSubTestGroups(_testsRoot, _testsPath);

      Tests.SelectedItemChanged += _testsSelectionChanged;
      Tests.Items.Add(_testsRoot);

      WindowState = System.Windows.WindowState.Maximized;
    }

    void _testsSelectionChanged(object s, RoutedPropertyChangedEventArgs<object> e) {
      selectedTestPath.Text = _selectedPath = _getPath(((TreeView)s).SelectedItem as TreeViewItem);
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

      passedLabel.Content = "Passed: " + passed;
      failedLabel.Content = "Failed: " + failed;
    }

    void run_Click(object sender, RoutedEventArgs e) {
      _testFiles.Clear();
      failedTests.Items.Clear();
      _collectTestFiles(_testsPath + "\\" + _selectedPath);
      _updateProgress(0, 0, 0);

      ThreadStart testThread = runTests;
      new Thread(testThread).Start();
    }

    void _addFailedTest(FailedTest test) {
      failedTests.Items.Add(test);
    }

    void Error(string error) {
      throw new Exception(error);
    }

    IronJS.Hosting.Context _createContext() {
      var ctx = IronJS.Hosting.Context.Create();
      var error = new Action<string>(Error);
      var errorFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, error);
      ctx.PutGlobal("ERROR", errorFunc);
      ctx.PutGlobal("$ERROR", errorFunc);
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

    bool _stop = false;
    void runTests() {
      _stop = false;

      int testsRun = 0;
      int passed = 0;
      int failed = 0;

      foreach (var file in _testFiles) {
        if (_stop)
          break;

        var ctx = _createContext();
        var test = File.ReadAllText(file);
        var shouldFail = test.Contains("@negative");

        try {
          ctx.Execute(test);

          if (!shouldFail) { ++passed; } else {
            ++failed;
            _failed(test, file, "Should Fail");
          }

        } catch (Exception ex) {

          if (shouldFail) { ++passed; } else {
            ++failed;
            _failed(test, file, _exTrace(ex).Trim());
          }

        }
        _updateProgress_Thread(++testsRun, passed, failed);
      }
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

  }
}
