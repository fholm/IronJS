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

namespace DebugConsole
{
    public partial class MainWindow : Window
    {
        const string CACHE_FILE = "input.cache";

        CallbackWriter callbackWriter;
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

            callbackWriter = new CallbackWriter(printConsoleText);
            Console.SetOut(callbackWriter);

            createEnvironment();
        }

        void loadCacheFile()
        {
            try
            {
                inputText.Text = File.ReadAllText(CACHE_FILE);
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

        IEnumerable<TreeViewItem> renderObjectProperties(IronJS.CommonObject ijsObject)
        {
            if (ijsObject != null && !alreadyRendered.Contains(ijsObject))
            {
                if (ijsObject.Prototype != null)
                {
                    yield return renderProperty("[[Prototype]]", ijsObject.Prototype);
                }

                if (ijsObject is IronJS.ValueObject)
                {
                    var value = (ijsObject as IronJS.ValueObject).Value.Value.ClrBoxed;
                    yield return renderProperty("[[Value]]", value);
                }

                alreadyRendered.Add(ijsObject);
                foreach (var member in ijsObject.Members)
                {
                    yield return renderProperty(member.Key, member.Value);
                }

                if (ijsObject is IronJS.ArrayObject)
                {
                    var arrayObject = ijsObject as IronJS.ArrayObject;
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

            var result = context.Execute(inputText.Text);

            lastStatementOutput.Text =
                IronJS.TypeConverter.ToString(IronJS.BoxingUtils.JsBox(result));

            printEnvironmentVariables(context.Globals);
        }

        void stopButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void inputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                File.WriteAllText(CACHE_FILE, inputText.Text);
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
            createEnvironment();
            printEnvironmentVariables(context.Globals);
        }
    }
}
