using System;
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
using System.Reflection;

namespace IronJS.DevUI {
    public static class Ext {
        static public bool SetSelectedItem(this TreeView treeView, object item) {
            return SetSelected(treeView, item);
        }

        static private bool SetSelected(ItemsControl parent,
            object child) {

            if (parent == null || child == null) {
                return false;
            }

            TreeViewItem childNode = parent.ItemContainerGenerator
                .ContainerFromItem(child) as TreeViewItem;

            if (childNode != null) {
                childNode.Focus();
                return childNode.IsSelected = true;
            }

            if (parent.Items.Count > 0) {
                foreach (object childItem in parent.Items) {
                    ItemsControl childControl = parent
                        .ItemContainerGenerator
                        .ContainerFromItem(childItem)
                        as ItemsControl;

                    if (SetSelected(childControl, child)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        IronJS.Hosting.Context ijsCtx;
        System.Diagnostics.Stopwatch stopWatch;
        Dictionary<IronJS.Object, TreeViewItem> printedObjects;

        public MainWindow() {
            InitializeComponent();
            Title = IronJS.Version.FullName + " DevUI";
            stopWatch = new System.Diagnostics.Stopwatch();
            printedObjects = new Dictionary<Object, TreeViewItem>();

            RunCode.Click += new RoutedEventHandler(RunCode_Click);
            ResetEnv.Click += new RoutedEventHandler(ResetEnv_Click);

            ResetEnv_Click(null, null);

            IronJS.Printer.print = new IronJS.Print(Print);

            if (System.IO.File.Exists("ironjs_devgui.cache")) {
                Input.Text = System.IO.File.ReadAllText("ironjs_devgui.cache");
            }
        }

        void Print(System.Linq.Expressions.Expression expr) {
            Debug.Text += IronJS.Dlr.Utils.debugView(expr);
        }

        void ResetEnv_Click(object sender, RoutedEventArgs e) {
            ijsCtx = IronJS.Hosting.Context.Create();
        }

        void RunCode_Click(object sender, RoutedEventArgs e) {
            System.IO.File.WriteAllText("ironjs_devgui.cache", Input.Text);

            stopWatch.Restart();
            Debug.Text = "";
            var result = ijsCtx.Execute(Input.Text);
            stopWatch.Stop();
            ExecutionTime.Content = stopWatch.ElapsedMilliseconds + "ms";
            Result.Text = IronJS.Api.TypeConverter.toString(result);

            printedObjects.Clear();
            Variables.Items.Clear();

            var globals = RenderIronJSObjectValues(ijsCtx.Environment.Globals);

            foreach (var item in globals) {
                Variables.Items.Add(item);
            }
        }

        List<TreeViewItem> RenderIronJSObjectValues(IronJS.Object obj) {
            var items = new List<TreeViewItem>();

            if (obj != null) {
                foreach (var kvp in obj.PropertyClass.PropertyMap) {
                    items.Add(
                            RenderIronJSValue(
                                kvp.Key, obj.PropertyValues[kvp.Value]));
                }
            }

            return items;
        }

        TreeViewItem RenderIronJSValue(string name, IronJS.Box box) {
            var item = new TreeViewItem();
            var header = item as HeaderedItemsControl;
            header.Header = name + ": " + IronJS.Api.TypeConverter.toString(box);

            switch (box.Type) {
                case IronJS.TypeCodes.Object:
                case IronJS.TypeCodes.Function:
                    if (printedObjects.ContainsKey(box.Object)) {
                        item = new TreeViewItem();
                        header = item as HeaderedItemsControl;
                        header.Header = name + ": <recursive>";

                        item.MouseUp += new MouseButtonEventHandler((s, e) => {
                            Variables.SetSelectedItem(printedObjects[box.Object]);
                        });

                        return item;

                    } else {
                        printedObjects.Add(box.Object, item);

                        if (box.Object.Prototype != null) {
                            item.Items.Add(RenderIronJSValue("[[Prototype]]", IronJS.Utils.boxObject(box.Object.Prototype)));
                        }

                        item.Items.Add(RenderIronJSValue("[[Value]]", box.Object.Value));

                        foreach (var child in RenderIronJSObjectValues(box.Object)) {
                            item.Items.Add(child);
                        }
                    }
                    break;
            }

            return item;
        }
    }
}
