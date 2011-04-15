using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Threading;
using IronJS;

namespace DebugConsole
{
    public partial class MainWindow : Window
    {
        const string CACHE_FILE = "input.cache";

        Dictionary<Type, Color> typeColors = new Dictionary<Type, Color>();
        HashSet<object> alreadyRendered = new HashSet<object>();
        IronJS.Hosting.CSharp.Context context;
        Thread jsThread;
        ManualResetEvent breakpointEvent = new ManualResetEvent(true);

        public MainWindow()
        {
            InitializeComponent();
            Width = 1280;
            Height = 720;

            loadCacheFile();

            IronJS.Support.Debug.registerExprPrinter(expressionTreePrinter);
            IronJS.Support.Debug.registerAstPrinter(syntaxTreePrinter);

            typeColors.Add(typeof(double), Colors.DarkOrchid);
            typeColors.Add(typeof(string), Colors.Brown);
            typeColors.Add(typeof(bool), Colors.DarkBlue);
            typeColors.Add(typeof(Undefined), Colors.DarkGoldenrod);
            typeColors.Add(typeof(CommonObject), Colors.DarkGreen);
            typeColors.Add(typeof(object), Colors.Black);

            Console.SetOut(new CallbackWriter(printConsoleText));

            createEnvironment();

            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (jsThread != null)
                jsThread.Abort();
        }

        void loadCacheFile()
        {
            try
            {
                inputText.TextChanged -= inputText_TextChanged;
                var lines = File.ReadLines(CACHE_FILE);
                inputText.Document = new FlowDocument();
                foreach (var line in lines)
                {
                    inputText.Document.Blocks.Add(new Paragraph(new Run(line)));
                }
                inputText.TextChanged += inputText_TextChanged;
            }
            catch
            {

            }
        }

        TextRange tr;
        void highlightBreakpoint(Run run)
        {
            if (run.Text.Trim().StartsWith("#bp"))
            {
                tr = new TextRange(run.ContentStart, run.ContentEnd);
                tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Red);
            }
        }

        void breakPoint(int line, int column, Dictionary<string, object> globals, Dictionary<string, object> scope)
        {
            breakpointEvent.Reset();

            Dispatcher.Invoke(new Action(() =>
                {
                    EnvironmentVariables.Items.Clear();

                    foreach (var kvp in globals)
                    {
                        alreadyRendered.Clear();
                        EnvironmentVariables.Items.Add(renderProperty(kvp.Key, kvp.Value));
                    }
                }));

            Dispatcher.Invoke(new Action(() =>
                {
                    Locals.Items.Clear();

                    foreach (var kvp in scope)
                    {
                        alreadyRendered.Clear();
                        Locals.Items.Add(renderProperty(kvp.Key, kvp.Value));
                    }

                    tabs.SelectedIndex = 4;
                }));

            Dispatcher.Invoke(new Action(() =>
                {
                    inputText.ScrollToHorizontalOffset(line);

                    if (inputText.Document == null)
                        return;

                    var run = 0;
                    var navigator = inputText.Document.ContentStart;

                    inputText.TextChanged -= inputText_TextChanged;

                    while (navigator.CompareTo(inputText.Document.ContentEnd) < 0)
                    {
                        var context = navigator.GetPointerContext(LogicalDirection.Backward);
                        if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                        {
                            ++run;
                            if (run == line)
                            {
                                highlightBreakpoint((Run)navigator.Parent);
                            }
                        }
                        navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                    }

                    inputText.TextChanged += inputText_TextChanged;
                }));

            breakpointEvent.WaitOne();
        }

        void expressionTreePrinter(string expressionTree)
        {
            Dispatcher.Invoke(new Action(() => expressionTreeOutput.Text += expressionTree));
        }

        void syntaxTreePrinter(string syntaxTree)
        {
            Dispatcher.Invoke(new Action(() => syntaxTreeOutput.Text += syntaxTree));
        }

        void printEnvironmentVariables(CommonObject globals)
        {
            EnvironmentVariables.Items.Clear();

            foreach (var item in renderObjectProperties(globals))
            {
                alreadyRendered.Clear();
                EnvironmentVariables.Items.Add(item);
            }
        }

        void printConsoleText(string value)
        {
            Dispatcher.Invoke(new Action(()=> consoleOutput.Text += value));
        }

        IEnumerable<TreeViewItem> renderObjectProperties(CommonObject jsObject)
        {
            if (jsObject != null && !alreadyRendered.Contains(jsObject))
            {
                if (jsObject.Prototype != null)
                {
                    yield return renderProperty("[[Prototype]]", jsObject.Prototype);
                }

                if (jsObject is ValueObject)
                {
                    var value = (jsObject as ValueObject).Value.Value.ClrBoxed;
                    yield return renderProperty("[[Value]]", value);
                }

                alreadyRendered.Add(jsObject);
                foreach (var member in jsObject.Members)
                {
                    yield return renderProperty(member.Key, member.Value);
                }

                if (jsObject is ArrayObject)
                {
                    var arrayObject = jsObject as ArrayObject;
                    for (var i = 0u; i < arrayObject.Length; ++i)
                    {
                        yield return renderProperty("[" + i + "]", arrayObject.Get(i).ClrBoxed);
                    }
                }
            }
        }

        TreeViewItem renderProperty(string name, object value)
        {
            Color color;

            var item = new TreeViewItem();
            var header = item as HeaderedItemsControl;

            if (!typeColors.TryGetValue(value.GetType(), out color))
            {
                if (value is CommonObject)
                {
                    color = typeColors[typeof(CommonObject)];
                }
                else
                {
                    color = typeColors[typeof(object)];
                }
            }

            header.Foreground = new SolidColorBrush(color);

            if (value is IronJS.CommonObject)
            {
                var commonObject = value as CommonObject;
                item.Header = name + ": " + commonObject.ClassName;

                if (alreadyRendered.Contains(value))
                {
                    item.Header += " <recursive>";
                }
                else
                {
                    foreach (var property in renderObjectProperties(commonObject))
                    {
                        item.Items.Add(property);
                    }
                }
            }
            else if (value is string)
            {
                item.Header = name + ": \"" + value + "\"";
            }
            else
            {
                item.Header = name + ": " + TypeConverter.ToString(BoxingUtils.JsBox(value));
            }

            return item;
        }

        string getAllText()
        {
            var tr = new TextRange(
                inputText.Document.ContentStart,
                inputText.Document.ContentEnd
            );

            return tr.Text;
        }

        void runButton_Click(object sender, RoutedEventArgs e)
        {
            consoleOutput.Text = String.Empty;
            expressionTreeOutput.Text = String.Empty;
            syntaxTreeOutput.Text = String.Empty;
            lastStatementOutput.Text = String.Empty;

            var input = getAllText();

            jsThread = new Thread(() => {
                try
                {
                    var result = context.Execute(input);
                    var asString = TypeConverter.ToString(BoxingUtils.JsBox(result));

                    Dispatcher.Invoke(new Action(() =>
                    {
                        consoleOutput.Text += "\r\nLast statement: " + asString;
                    }));
                }
                catch (Exception exn)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        tabs.SelectedIndex = 3;
                        lastStatementOutput.Text = exn.ToString();
                    }));
                }
                finally
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        printEnvironmentVariables(context.Globals);
                    }));
                }
            });

            jsThread.Start();
        }

        void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (tr != null)
                tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);

            breakpointEvent.Set();
        }

        void inputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            File.WriteAllText(CACHE_FILE, getAllText());
        }

        void createEnvironment()
        {
            context = new IronJS.Hosting.CSharp.Context();
            context.CreatePrintFunction();
            context.Environment.BreakPoint = breakPoint;
        }

        void resetEnvironment_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 2;
            createEnvironment();
            printEnvironmentVariables(context.Globals);
        }
    }
}
