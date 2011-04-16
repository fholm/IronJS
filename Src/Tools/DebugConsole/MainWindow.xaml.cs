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

        HashSet<object> alreadyRendered = new HashSet<object>();
        ManualResetEvent breakpointEvent = new ManualResetEvent(true);
        Dictionary<Type, Color> typeColors = new Dictionary<Type, Color>();

        Thread jsThread;
        TextRange currentHighlight;
        IronJS.Hosting.CSharp.Context context;

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
                inputText.Document = new FlowDocument();

                foreach (var line in File.ReadLines(CACHE_FILE))
                {
                    inputText.Document.Blocks.Add(new Paragraph(new Run(line)));
                }

                inputText.TextChanged += inputText_TextChanged;
            }
            catch
            {

            }
        }

        void printLocalVariables(Dictionary<string, object> locals)
        {
            Locals.Items.Clear();

            foreach (var kvp in locals)
            {
                alreadyRendered.Clear();
                Locals.Items.Add(renderProperty(kvp.Key, kvp.Value));
            }

            tabs.SelectedIndex = 4;
        }

        void highlightBreakpoint(Run run)
        {
            if (run.Text.Trim().StartsWith("#bp"))
            {
                currentHighlight = new TextRange(run.ContentStart, run.ContentEnd);
                currentHighlight.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Salmon);
                var position = run.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                inputScroller.ScrollToVerticalOffset(position.Top);
            }
        }

        void findAndHighlightCurrentBreakpoint(int line)
        {
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
                        break;
                    }
                }

                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

            inputText.TextChanged += inputText_TextChanged;
        }

        void breakPoint(int line, int column, Dictionary<string, object> scope)
        {
            //Reset breakpoint event
            breakpointEvent.Reset();

            Dispatcher.Invoke(new Action(() => printEnvironmentVariables(context.Globals)));
            Dispatcher.Invoke(new Action(() => printLocalVariables(scope)));
            Dispatcher.Invoke(new Action(() => findAndHighlightCurrentBreakpoint(line)));

            //Wait for UI thread to set event
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
            Dispatcher.Invoke(new Action(() => consoleOutput.Text += value));
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

            if (value is CommonObject)
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

        string getAllInputText()
        {
            var tr = new TextRange(
                inputText.Document.ContentStart,
                inputText.Document.ContentEnd
            );

            return tr.Text;
        }

        void printException(Exception exn)
        {
            tabs.SelectedIndex = 3;
            lastStatementOutput.Text = exn.ToString();
        }

        void runButton_Click(object sender, RoutedEventArgs e)
        {
            if (jsThread != null)
                return;

            consoleOutput.Text = String.Empty;
            expressionTreeOutput.Text = String.Empty;
            syntaxTreeOutput.Text = String.Empty;
            lastStatementOutput.Text = String.Empty;
            inputText.Focusable = false;

            var input = getAllInputText();

            jsThread = new Thread(() => {
                try
                {
                    var result = context.Execute(input);
                    var resultAsString = TypeConverter.ToString(BoxingUtils.JsBox(result));

                    Dispatcher.Invoke(new Action(() => 
                        consoleOutput.Text += "\r\nLast statement: " + resultAsString));
                }
                catch (Exception exn)
                {
                    Dispatcher.Invoke(new Action(() => 
                        printException(exn)));
                }
                finally
                {
                    Dispatcher.Invoke(new Action(() => 
                        printEnvironmentVariables(context.Globals)));

                    jsThread = null;
                    inputText.Focusable = true;
                }
            });

            jsThread.Start();
        }

        void continueButton_Click(object sender, RoutedEventArgs e)
        {
            // We need to reset the color of the
            // previously hit breakpoint textrange
            if (currentHighlight != null)
            {
                currentHighlight.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
            }

            // Allow the executing thread to continue
            breakpointEvent.Set();
        }

        void inputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            File.WriteAllText(CACHE_FILE, getAllInputText());
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
