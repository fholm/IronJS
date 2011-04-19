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
using WinFormsRichhTextBox = System.Windows.Forms.RichTextBox;

namespace DebugConsole
{
    public partial class MainWindow : Window
    {
        const string CACHE_FILE = "source.cache";
        const string CACHE_FILE_LIST = "files.cache";

        HashSet<object> alreadyRendered = new HashSet<object>();
        ManualResetEvent breakpointEvent = new ManualResetEvent(true);
        Dictionary<Type, Color> typeColors = new Dictionary<Type, Color>();
        List<TextRange> currentBreakPoints = new List<TextRange>();

        Thread jsThread;
        TextRange currentHighlight;
        IronJS.Hosting.CSharp.Context context;
        string lastFileBrowserPath = null;

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
            DataObject.AddPastingHandler(inputText, inputText_OnPaste);

            createEnvironment();

            this.Closing += MainWindow_Closing;

            loadFilesCache();
        }

        void loadFilesCache()
        {
            try
            {
                var files = File.ReadAllLines(CACHE_FILE_LIST);
                foreach (var file in files)
                {
                    if (file.Trim() != "")
                    {
                        addNewFilePanel(file);
                    }
                }
            }
            catch
            {
            }
        }

        void inputText_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            // This little hack removes the RTF formatting on the input text

            if (!e.SourceDataObject.GetDataPresent(DataFormats.Rtf, true)) 
                return;

            var rtf = e.SourceDataObject.GetData(DataFormats.Rtf) as string;
            var rtb = new WinFormsRichhTextBox();
            rtb.Rtf = rtf;

            e.DataObject = new DataObject(DataFormats.UnicodeText, rtb.Text);
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
                doWithoutTextChange(() =>
                {
                    inputText.Document = new FlowDocument();

                    foreach (var line in File.ReadLines(CACHE_FILE))
                    {
                        inputText.Document.Blocks.Add(new Paragraph(new Run(line)));
                    }
                });
            }
            catch
            {

            }
        }

        void displayLocalVariables(Dictionary<string, object> locals)
        {
            Locals.Items.Clear();

            foreach (var kvp in locals)
            {
                alreadyRendered.Clear();
                Locals.Items.Add(renderProperty(kvp.Key, kvp.Value));
            }

            tabs.SelectedIndex = 4;
        }

        void displayGlobalVariables(CommonObject globals)
        {
            EnvironmentVariables.Items.Clear();

            foreach (var item in renderObjectProperties(globals))
            {
                alreadyRendered.Clear();
                EnvironmentVariables.Items.Add(item);
            }
        }

        void highlightBreakpoint(Run run, Brush brush)
        {
            if (run.Text.Trim().StartsWith("#bp"))
            {
                currentHighlight = new TextRange(run.ContentStart, run.ContentEnd);
                currentHighlight.ApplyPropertyValue(TextElement.BackgroundProperty, brush);
                var position = run.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                inputScroller.ScrollToVerticalOffset(position.Top);
                currentBreakPoints.Add(currentHighlight);
            }
        }

        void doForEachLine(Action<Run> foreachLine)
        {
            if (inputText.Document == null)
                return;

            doWithoutTextChange(() =>
            {
                var navigator = inputText.Document.ContentStart;

                while (navigator.CompareTo(inputText.Document.ContentEnd) < 0)
                {
                    var context = navigator.GetPointerContext(LogicalDirection.Backward);
                    if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                    {
                        foreachLine(navigator.Parent as Run);
                    }

                    navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                }
            });
        }

        void highlightActiveBreakPoint(int line)
        {
            int currentLine = 0;

            doForEachLine((run) =>
            {
                ++currentLine;
                if (currentLine == line)
                {
                    highlightBreakpoint(run, Brushes.Salmon);
                }
            });
        }

        void breakPoint(int line, int column, Dictionary<string, object> scope)
        {
            //Play a sound
            System.Media.SystemSounds.Beep.Play();

            //Reset breakpoint event
            breakpointEvent.Reset();

            //
            Dispatcher.Invoke(new Action(() =>
            {
                breakPointLabel.Content = "Hit breakpoint on line " + line;
                continueButton.Visibility = Visibility.Visible;
                displayGlobalVariables(context.Globals);
                displayLocalVariables(scope);
                highlightActiveBreakPoint(line);
            }));

            //Wait for UI thread to set event
            breakpointEvent.WaitOne();

            //
            Dispatcher.Invoke(new Action(() =>
            {
                breakPointLabel.Content = "Running...";
                continueButton.Visibility = Visibility.Collapsed;
            }));
        }

        void expressionTreePrinter(string expressionTree)
        {
            Dispatcher.Invoke(new Action(() => expressionTreeOutput.Text += expressionTree));
        }

        void syntaxTreePrinter(string syntaxTree)
        {
            Dispatcher.Invoke(new Action(() => syntaxTreeOutput.Text += syntaxTree));
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

            if (value != null && !typeColors.TryGetValue(value.GetType(), out color))
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
            else
            {
                color = typeColors[typeof(object)];
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

        void doWithoutTextChange(Action action)
        {
            inputText.TextChanged -= inputText_TextChanged;
            action();
            inputText.TextChanged += inputText_TextChanged;
        }

        void runButton_Click(object sender, RoutedEventArgs e)
        {
            runSource(getAllInputText());
        }

        void runSource(string source)
        {
            consoleOutput.Text = String.Empty;
            expressionTreeOutput.Text = String.Empty;
            syntaxTreeOutput.Text = String.Empty;
            lastStatementOutput.Text = String.Empty;

            var sources = new Stack<string>();
            sources.Push(source);
            runSources(sources);
        }

        void runSources(Stack<string> sources)
        {
            if (jsThread != null)
                return;

            doWithoutTextChange(() =>
            {
                inputText.Focusable = false;
                inputText.Background = Brushes.WhiteSmoke;
            });

            jsThread = new Thread(() =>
            {
                try
                {
                    if (sources.Count > 0)
                    {
                        Dispatcher.Invoke(new Action(() => breakPointLabel.Content = "Running..."));

                        var result = context.Execute(sources.Pop());
                        var resultAsString = TypeConverter.ToString(BoxingUtils.JsBox(result));

                        Dispatcher.Invoke(new Action(() =>
                            consoleOutput.Text += "\r\nLast statement: " + resultAsString));
                    }
                }
                catch (Exception exn)
                {
                    Dispatcher.Invoke(new Action(() => printException(exn)));
                }
                finally
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        doWithoutTextChange(() =>
                        {
                            inputText.Focusable = true;
                            inputText.Background = Brushes.Transparent;
                        });

                        displayGlobalVariables(context.Globals);
                    }));

                    jsThread = null;

                    if (sources.Count > 0)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            runSources(sources);
                        }));
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(() => breakPointLabel.Content = ""));
                    }
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
            displayGlobalVariables(context.Globals);
        }

        List<string> getAllLoadedFiles()
        {
            var files = new List<string>();

            foreach (var child in filesPanel.Children)
            {
                if (child is StackPanel)
                {
                    foreach (var subChild in (child as StackPanel).Children)
                    {
                        if (subChild is TextBox)
                        {
                            var tb = subChild as TextBox;
                            if (tb.Text != null && tb.Text.Trim() != "")
                            {
                                files.Add(tb.Text.Trim() + "\r\n");
                            }
                        }
                    }
                }
            }

            return files;
        }

        void saveFileListCache()
        {
            File.WriteAllText(CACHE_FILE_LIST, String.Concat(getAllLoadedFiles()));
        }

        void addNewFileButton_Click(object sender, RoutedEventArgs e)
        {
            addNewFilePanel(null);
        }

        void addNewFilePanel(string initialFile)
        {
            var panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;

            var removeButton = new Button();
            removeButton.Content = "Remove";
            removeButton.Width = 60;
            removeButton.Margin = new Thickness(5);

            var filePathBox = new TextBox();
            filePathBox.Width = 400;
            filePathBox.Margin = new Thickness(5);

            var browseButton = new Button();
            browseButton.Content = "Browse";
            browseButton.Width = 50;
            browseButton.Margin = new Thickness(5);

            var runButton = new Button();
            runButton.Content = "Run";
            runButton.Width = 50;
            runButton.Margin = new Thickness(5);

            panel.Children.Add(removeButton);
            panel.Children.Add(filePathBox);
            panel.Children.Add(browseButton);
            panel.Children.Add(runButton);

            filesPanel.Children.Add(panel);

            removeButton.Click += (s, args) =>
            {
                filesPanel.Children.Remove(panel);
                saveFileListCache();
            };

            browseButton.Click += (s, args) =>
            {
                var dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".js";

                if (!String.IsNullOrEmpty(lastFileBrowserPath))
                {
                    dlg.InitialDirectory = lastFileBrowserPath;
                }

                if (dlg.ShowDialog() ?? false)
                {
                    lastFileBrowserPath = System.IO.Path.GetDirectoryName(dlg.FileName);
                    filePathBox.Text = dlg.FileName;
                }

                saveFileListCache();
            };

            runButton.Click += (s, args) =>
            {
                var path = (filePathBox.Text ?? "").Trim();

                if (path != "")
                {
                    try
                    {
                        runSource(File.ReadAllText(path));
                    }
                    catch
                    {

                    }
                }
            };

            if (initialFile != null)
            {
                filePathBox.Text = initialFile;
            }
        }

        void runAllFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var sources = new Stack<string>(getAllLoadedFiles().Reverse<string>() .Select(x => File.ReadAllText(x.Trim())));
            runSources(sources);
        }
    }
}
