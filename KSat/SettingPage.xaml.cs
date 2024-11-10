using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class SettingPage : Page
    {
        public SettingPage()
        {

            
            //TLEData.GetTLEDataFromWeb(25544);
            this.InitializeComponent();
            UpdateUITime(GetLastUpdateTime());
            InitializeAsync();

        }
        private async Task InitializeAsync()
        {
            int count = await TLEData.getSatsCount(); // 使用 await 调用异步方法
            SatsCount.Text = "Satellites Count: " + count;
        }

        private String GetLastUpdateTime()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey("last_update_time")) return "Never";
            String time = (String)localSettings.Values["last_update_time"];
            return time;
        }

        private void UpdateUITime(String time)
        {
            UpdateTime.Text = "Last Update from Web:\n" + time;
        }

        private void SetLastUpdateTime(DateTime time)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["last_update_time"] = time.ToString();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {
           
        }

        private async void Get_web_Click(object sender, RoutedEventArgs e)
        {
            LoadingRing.IsActive = true;
            LoadingRing.Visibility = Visibility.Visible;

            try
            {
               await TLEData.GetTLEDataFromWeb();
            }
            finally
            {
                SetLastUpdateTime(DateTime.Now);
                UpdateUITime(GetLastUpdateTime());
                await InitializeAsync();
                LoadingRing.IsActive = false;
                LoadingRing.Visibility = Visibility.Collapsed;
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Select_display_button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SelectSat));
        }
    }
}
