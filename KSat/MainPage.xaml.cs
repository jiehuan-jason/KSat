using SGPdotNET.CoordinateSystem;
using SGPdotNET.Util;
using System;
using System.Collections.Generic;
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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace KSat
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<SimpleSatData> PassDataLists;
        private SimpleSatData CurrentCategory;
        public MainPage()
        {
            this.InitializeComponent();

            showSatDataList();
        }

        private async void showSatDataList()
        {
            ShowLoading(true);
            var localData = await new SimpleSatData("", 0).GetSatDataLocalAsync();

            PassDataLists = new List<SimpleSatData>();
            foreach(var sat in localData)
            {
                PassDataLists.Add(sat);
            }

            // 默认显示类别列表
            CategoryListView.ItemsSource = PassDataLists;
            ShowLoading(false);
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage));
        }

        private void CategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryListView.SelectedItem is SimpleSatData selectedSatellite)
            {
                // 导航到 SatPassListPage 并传递数据
                Frame.Navigate(typeof(SatPassListPage), selectedSatellite);
            }
        }
        private void ShowLoading(bool isLoading)
        {
            LoadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
