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
using System.Net.NetworkInformation;

namespace CoreBenchmarks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string Identity
        {
            get
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                var interfaceIds = interfaces.Where(
                            x => 
                                x.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                                || x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                        ).Select(x => x.Id).ToArray();

                return String.Join("#", interfaceIds);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            var nics = Identity;
        }
    }
}
