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

namespace DebugConsole
{
    public partial class MainWindow : Window
    {
        const string CACHE_FILE = "input.cache";

        Dictionary<Type, Color> typeColors = new Dictionary<Type, Color>();
        HashSet<object> alreadyRendered = new HashSet<object>();
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
            typeColors.Add(typeof(IronJS.Undefined), Colors.DarkGoldenrod);
            typeColors.Add(typeof(IronJS.CommonObject), Colors.DarkGreen);
            typeColors.Add(typeof(object), Colors.Black);

            Console.SetOut(new CallbackWriter(printConsoleText));

            createEnvironment();
        }

        void loadCacheFile()
        {
            try
            {
                var text = File.ReadLines(CACHE_FILE);
                var doc = new FlowDocument();
                doc.PageWidth = 1000;

                foreach (var t in text)
                {
                    doc.Blocks.Add(new Paragraph(new Run(t)));
                }

                inputText.Document = doc;
            }
            catch
            {

            }
        }

        void expressionTreePrinter(string expressionTree)
        {
            expressionTreeOutput.Text += expressionTree;
        }

        void syntaxTreePrinter(string syntaxTree)
        {
            syntaxTreeOutput.Text += syntaxTree;
        }

        void printEnvironmentVariables(IronJS.CommonObject globals)
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
            consoleOutput.Text += value;
        }

        IEnumerable<TreeViewItem> renderObjectProperties(IronJS.CommonObject jsObject)
        {
            if (jsObject != null && !alreadyRendered.Contains(jsObject))
            {
                if (jsObject.Prototype != null)
                {
                    yield return renderProperty("[[Prototype]]", jsObject.Prototype);
                }

                if (jsObject is IronJS.ValueObject)
                {
                    var value = (jsObject as IronJS.ValueObject).Value.Value.ClrBoxed;
                    yield return renderProperty("[[Value]]", value);
                }

                alreadyRendered.Add(jsObject);
                foreach (var member in jsObject.Members)
                {
                    yield return renderProperty(member.Key, member.Value);
                }

                if (jsObject is IronJS.ArrayObject)
                {
                    var arrayObject = jsObject as IronJS.ArrayObject;
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
                if (value is IronJS.CommonObject)
                {
                    color = typeColors[typeof(IronJS.CommonObject)];
                }
                else
                {
                    color = typeColors[typeof(object)];
                }
            }

            header.Foreground = new SolidColorBrush(color);

            if (value is IronJS.CommonObject)
            {
                var commonObject = value as IronJS.CommonObject;
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
                item.Header = name + ": " + IronJS.TypeConverter.ToString(IronJS.BoxingUtils.JsBox(value));
            }

            return item;
        }

        void runButton_Click(object sender, RoutedEventArgs e)
        {
            consoleOutput.Text = String.Empty;
            expressionTreeOutput.Text = String.Empty;
            syntaxTreeOutput.Text = String.Empty;
            lastStatementOutput.Text = String.Empty;

            try
            {
                var result = 0.0; //context.Execute(inputText.Text);

                lastStatementOutput.Text =
                    IronJS.TypeConverter.ToString(IronJS.BoxingUtils.JsBox(result));

            }
            catch (Exception exn) 
            {
                tabs.SelectedIndex = 3;
                lastStatementOutput.Text = exn.ToString();
            }
            finally
            {
                printEnvironmentVariables(context.Globals);
            }
        }

        void stopButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        List<string> lineContents = new List<string>();
        void inputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var documentRange = new TextRange(inputText.Document.ContentStart, inputText.Document.ContentEnd);
            File.WriteAllText(CACHE_FILE, documentRange.Text);

            var lines = Regex.Split(documentRange.Text, "\r\n");
            var lineCount = lines.Length - 1;
            var newLineContents = new List<string>();

            var bps = new object[breakPoints.Items.Count];
            breakPoints.Items.CopyTo(bps, 0);
            breakPoints.Items.Clear();

            for (var i = 0; i < lineCount; ++i)
            {
                CheckBox current = null;

                if (i < bps.Length && i < lineContents.Count)
                {
                    current = bps[i] as CheckBox;
                    if (current != null && lineContents[i].Trim() == lines[i].Trim())
                    {
                        breakPoints.Items.Add(bps[i]);
                        continue;
                    }
                }

                newLineContents.Add(lines[i]);
                breakPoints.Items.Add(createBreakPointCheckBox(lines[i]));
            }

            lineContents = newLineContents;
            highlightBreakpoints(null, null);
        }

        object createBreakPointCheckBox(string text)
        {
            text = text.Trim();

            var checkbox = new CheckBox();
            var isHidden = text == "" || text.StartsWith("//");

            checkbox.Visibility = isHidden ? Visibility.Hidden : Visibility.Visible;
            checkbox.Margin = new Thickness(0, 1, 0, 0);
            checkbox.Checked += (sender, args) => highlightBreakpoints(null, null);
            checkbox.Unchecked += (sender, args) => highlightBreakpoints(null, null);

            return checkbox;
        }

        void highlightBreakpoints(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (inputText.Document == null)
                    return;

                inputText.TextChanged -= inputText_TextChanged;


                var documentRange = new TextRange(inputText.Document.ContentStart, inputText.Document.ContentEnd);
                documentRange.ClearAllProperties();

                var navigator = inputText.Document.ContentStart;
                var line = 0;
                while (navigator.CompareTo(inputText.Document.ContentEnd) < 0)
                {
                    var context = navigator.GetPointerContext(LogicalDirection.Backward);
                    if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                    {
                        if (line < breakPoints.Items.Count)
                        {
                            var bp = breakPoints.Items[line] as CheckBox;

                            if (bp.IsChecked ?? false)
                            {
                                var run = navigator.Parent as Run;
                                var startPosition = run.ContentStart.GetPositionAtOffset(0, LogicalDirection.Forward);
                                var endPosition = run.ContentStart.GetPositionAtOffset(run.Text.Length, LogicalDirection.Backward);
                                var range = new TextRange(startPosition, endPosition);
                                range.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Color.FromRgb(255, 174, 174)));
                            }

                            ++line;
                        }
                    }

                    navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                }
                
                inputText.TextChanged += inputText_TextChanged;
            }
            catch
            {

            }
        }

        void createEnvironment()
        {
            context = new IronJS.Hosting.CSharp.Context();
            context.CreatePrintFunction();
        }

        void resetEnvironment_Click(object sender, RoutedEventArgs e)
        {
            tabs.SelectedIndex = 2;
            createEnvironment();
            printEnvironmentVariables(context.Globals);
        }
    }
}
