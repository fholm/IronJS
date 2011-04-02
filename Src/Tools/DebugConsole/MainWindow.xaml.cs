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

        public MainWindow()
        {
            InitializeComponent();
            Width = 1280;
            Height = 720;

            loadCacheFile();

            IronJS.Support.Debug.registerExprPrinter(expressionTreePrinter);
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
            outputText.Text += expressionTree;
        }

        void runButton_Click(object sender, RoutedEventArgs e)
        {
            outputText.Text = String.Empty;
            var ctx = new IronJS.Hosting.CSharp.Context();
            ctx.Execute(inputText.Text);
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
    }
}
