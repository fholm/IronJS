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

    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window {

        IronJS.Hosting.Context ijsCtx;
        System.Diagnostics.Stopwatch stopWatch;
        Dictionary<IronJS.CommonObject, TreeViewItem> printedObjects;

        public MainWindow() {
            InitializeComponent();
            Title = IronJS.Version.FullName + " DevUI";

            stopWatch = new System.Diagnostics.Stopwatch();
            printedObjects = new Dictionary<CommonObject, TreeViewItem>();

            RunCode.Click += new RoutedEventHandler(RunCode_Click);
            ResetEnv.Click += new RoutedEventHandler(ResetEnv_Click);

            ResetEnv_Click(null, null);

            IronJS.Debug.exprPrinters.Add(Print);
            IronJS.Debug.stringPrinters.Add(Print);

            if (System.IO.File.Exists("ironjs_devgui.cache")) {
                Input.Text = System.IO.File.ReadAllText("ironjs_devgui.cache");
            }
        }

        void Print(string expr) {
            Debug.Text += expr;
        }

        void Print(System.Linq.Expressions.Expression expr) {
            ExpressionTree.Text += IronJS.Dlr.Utils.debugView(expr);
        }

        void ResetEnv_Click(object sender, RoutedEventArgs e) {
            ijsCtx = IronJS.Hosting.Context.Create();

            var inspect =
                IronJS.Api.HostFunction.create<Action<IronJS.BoxedValue>>(
                    ijsCtx.Environment, Inspect);

            var print =
                IronJS.Api.HostFunction.create<Action<IronJS.BoxedValue>>(
                    ijsCtx.Environment, Print);

            ijsCtx.PutGlobal("inspect", inspect);
            ijsCtx.PutGlobal("print", print);

            if (sender != null)
                RenderEnvironment();
        }

        void RenderEnvironment() {
            Variables.Items.Clear();

            var globals = RenderIronJSPropertyValues(ijsCtx.Environment.Globals, true);

            foreach (var item in globals) {
                Variables.Items.Add(item);
            }
        }

        void Inspect(IronJS.BoxedValue box) {
            return;
        }

        void Print(IronJS.BoxedValue box) {
            Result.Text += IronJS.TypeConverter2.ToString(box) + "\r\n";
        }

        void RunCode_Click(object sender, RoutedEventArgs e) {
            try {
                System.IO.File.WriteAllText("ironjs_devgui.cache", Input.Text);

                stopWatch.Restart();
                Debug.Text = "";
                ExpressionTree.Text = "";
                Result.Text = "";
                var result = Utils.box(ijsCtx.Execute(Input.Text));
                stopWatch.Stop();
                Result.Text += IronJS.TypeConverter2.ToString(result);

              /*} catch (Exception ex) {
                  stopWatch.Stop();

                  while (ex.InnerException != null) {
                      ex = ex.InnerException;
                  }

                  Result.Text = ex.Message + "\n\n";
                  Result.Text += ex.StackTrace;*/

            } finally {
                RenderEnvironment();
                ExecutionTime.Content = stopWatch.ElapsedMilliseconds + "ms";
            }

        }

        List<TreeViewItem> RenderIronJSPropertyValues(IronJS.CommonObject obj, bool isGlobal) {
            var items = new List<TreeViewItem>();

            if (obj != null) {
                foreach (var kvp in obj.PropertyMap.IndexMap) {
                  if (obj.Properties[kvp.Value].HasValue) {
                    if (isGlobal) {
                      printedObjects.Clear();
                    }

                    items.Add(
                      RenderIronJSValue(
                          kvp.Key, obj.Properties[kvp.Value].Value));

                  }
                }

                if (obj is ArrayObject) {
                    var arr = obj as ArrayObject;
                    for (var i = 0u; i < arr.Length; ++i) {
                        items.Add(RenderIronJSValue("[" + i + "]", arr.Get(i)));
                    }
                }
            }

            return items;
        }

        TreeViewItem RenderIronJSValue(string name, IronJS.BoxedValue box) {
            var item = new TreeViewItem();
            var header = item as HeaderedItemsControl;
            var isNum = IronJS.Utils.Box.isNumber(box.Marker);

            if (box.IsNumber) {
                header.Header = name + ": " + IronJS.TypeConverter2.ToString(box);
                item.Foreground = new SolidColorBrush(Colors.DarkOrchid);

            } else {

                switch (box.Tag) {
                    case IronJS.TypeTags.Undefined:
                        header.Header = name + ": " + IronJS.TypeConverter2.ToString(box);
                        item.Foreground = new SolidColorBrush(Colors.DarkGoldenrod);
                        break;

                    case IronJS.TypeTags.Bool:
                        header.Header = name + ": " + IronJS.TypeConverter2.ToString(box);
                        item.Foreground = new SolidColorBrush(Colors.DarkBlue);
                        break;

                    case IronJS.TypeTags.String:
                        item.Foreground = new SolidColorBrush(Colors.Brown);
                        header.Header = name + ": \"" + IronJS.TypeConverter2.ToString(box) + "\"";
                        break;

                    case IronJS.TypeTags.Object:
                    case IronJS.TypeTags.Function:
                        header.Header = name + ": " + IronJS.Classes.getName(box.Object.Class);
                        item.Foreground = new SolidColorBrush(Colors.DarkGreen);
                        if (printedObjects.ContainsKey(box.Object)) {
                            item = new TreeViewItem();
                            header = item as HeaderedItemsControl;
                            header.Header = name + ": <recursive>";
                            var obj = printedObjects[box.Object];
                            item.MouseUp += new MouseButtonEventHandler((s, e) => {
                                Variables.SetSelectedItem(obj);
                            });

                            item.Foreground = new SolidColorBrush(Colors.DarkGreen);

                            return item;

                        } else {
                            printedObjects.Add(box.Object, item);

                            if (box.Object.Prototype != null) {
                                item.Items.Add(RenderIronJSValue("[[Prototype]]", IronJS.Utils.boxObject(box.Object.Prototype)));
                            }

                            if (box.Object is IronJS.ValueObject) {
                                if (IronJS.Utils.Descriptor.hasValue((box.Object as ValueObject).Value)) {
                                  item.Items.Add(RenderIronJSValue("[[Value]]", (box.Object as ValueObject).Value.Value));
                                }
                            }

                            foreach (var child in RenderIronJSPropertyValues(box.Object, false)) {
                                item.Items.Add(child);
                            }
                        }
                        break;
                }
            }

            return item;
        }

        void QuitButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }

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
}
