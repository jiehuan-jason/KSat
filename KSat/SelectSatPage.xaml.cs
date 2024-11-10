using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KSat
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SelectSat : Page
    {
        public List<string> SatList { get; set; }
        public SelectSat()
        {
            this.InitializeComponent();
            SatList = new List<string>();
            this.DataContext = this;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            var tleList = await TLEData.getLocalTLEs();
            SatList = new List<string>();
            var results = tleList.Where(tle => tle.Name.Contains(textbox.Text.ToUpper())).ToList();
            if (results.Any())
            {
                foreach (var tle in results)
                {
                    Debug.WriteLine($"Found: Name = {tle.Name}");
                    
                    SatList.Add(tle.Name);
                }
            }
            else
            {
                SatList.Add("No result.");
                Debug.WriteLine($"No Tle with Name containing '{textbox.Text}' found.");
            }
            sat_list.ItemsSource = null;
            sat_list.ItemsSource = SatList;

        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
