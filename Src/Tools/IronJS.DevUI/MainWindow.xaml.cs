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

      IronJS.Debug.exprPrinters.Add(Print);

      if (System.IO.File.Exists("ironjs_devgui.cache")) {
        Input.Text = System.IO.File.ReadAllText("ironjs_devgui.cache");
      }
    }

    void Print(System.Linq.Expressions.Expression expr) {
      Debug.Text += IronJS.Dlr.Utils.debugView(expr);
    }

    void ResetEnv_Click(object sender, RoutedEventArgs e) {
      ijsCtx = IronJS.Hosting.Context.Create();

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

    void RunCode_Click(object sender, RoutedEventArgs e) {
      try {
        System.IO.File.WriteAllText("ironjs_devgui.cache", Input.Text);

        stopWatch.Restart();
        Debug.Text = "";
        var result = Utils.box(ijsCtx.Execute(Input.Text));
        stopWatch.Stop();
        Result.Text = IronJS.Api.TypeConverter.toString(ref result);

      } catch (Exception ex) {
        stopWatch.Stop();

        while (ex.InnerException != null) {
          ex = ex.InnerException;
        }

        Result.Text = ex.Message + "\n\n";
        Result.Text += ex.StackTrace;

      } finally {
        RenderEnvironment();
        ExecutionTime.Content = stopWatch.ElapsedMilliseconds + "ms";
      }

    }

    List<TreeViewItem> RenderIronJSPropertyValues(IronJS.Object obj, bool isGlobal) {
      var items = new List<TreeViewItem>();

      if (obj != null) {
          foreach (var kvp in obj.PropertyMap.PropertyMap) {
              if (isGlobal) {
                  printedObjects.Clear();
              }

              items.Add(
                RenderIronJSValue(
                    kvp.Key, obj.PropertyDescriptors[kvp.Value].Box));
          }

        if (Utils.isDense(obj)) {
          var length = obj.IndexLength;
          for (var i = 0u; i < length; ++i) {
            if (isGlobal) {
              printedObjects.Clear();
            }

            items.Add(RenderIronJSValue("[" + i + "]", obj.IndexDense[(int)i].Box));
          }
        } else {
          foreach (var kvp in obj.IndexSparse) {
            if (isGlobal) {
              printedObjects.Clear();
            }

            items.Add(RenderIronJSValue("[" + kvp.Key + "]", kvp.Value));
          }
        }
      }

      return items;
    }

    TreeViewItem RenderIronJSValue(string name, IronJS.Box box) {
      var item = new TreeViewItem();
      var header = item as HeaderedItemsControl;

      if (IronJS.Utils.Box.isNumber(box.Marker)) {
        header.Header = name + ": " + IronJS.Api.TypeConverter.toString(ref box);
        item.Foreground = new SolidColorBrush(Colors.DarkOrchid);

      } else {

        switch (box.Type) {
          case IronJS.TypeCodes.Undefined:
          case IronJS.TypeCodes.Empty:
            header.Header = name + ": " + IronJS.Api.TypeConverter.toString(ref box);
            item.Foreground = new SolidColorBrush(Colors.DarkGoldenrod);
            break;

          case IronJS.TypeCodes.Bool:
            header.Header = name + ": " + IronJS.Api.TypeConverter.toString(ref box);
            item.Foreground = new SolidColorBrush(Colors.DarkBlue);
            break;

          case IronJS.TypeCodes.String:
            item.Foreground = new SolidColorBrush(Colors.Brown);
            header.Header = name + ": \"" + IronJS.Api.TypeConverter.toString(ref box) + "\"";
            break;

          case IronJS.TypeCodes.Object:
          case IronJS.TypeCodes.Function:
            header.Header = name + ": " + IronJS.Api.TypeConverter.toString(ref box);
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

              if (IronJS.Utils.Descriptor.hasValue(box.Object.Value)) {
                item.Items.Add(RenderIronJSValue("[[Value]]", box.Object.Value.Box));
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
