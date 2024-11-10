using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace KSat
{
    public sealed partial class SelectSat : Page
    {
        public ObservableCollection<SimpleSatData> SatList { get; set; }
        public List<int> SatNumList { get; set; }

        public SelectSat()
        {
            this.InitializeComponent();
            SatList = new ObservableCollection<SimpleSatData>();
            SatNumList = new List<int>();
            this.DataContext = this;
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            LoadingRing.IsActive = true;
            LoadingRing.Visibility = Visibility.Visible;
            try
            {
                var tleList = await TLEData.getLocalTLEs();
                SatList.Clear();
                SatNumList.Clear();

                var results = tleList.Where(tle => tle.Name.Contains(textbox.Text.ToUpper())).ToList();

                if (results.Any())
                {
                    foreach (var tle in results)
                    {
                        // 创建 SimpleSatData 对象
                        var sat = new SimpleSatData(tle.Name, Convert.ToInt32(tle.NoradNumber));

                        // 初始化选中状态
                        await sat.InitializeSelectionStatusAsync();

                        SatList.Add(sat);
                        SatNumList.Add(sat.NoradNum);

                        Debug.WriteLine($"Found: Name = {tle.Name}, Selected = {sat.IsSelected}");
                    }
                }
                else
                {
                    SatList.Add(new SimpleSatData("No result", 0));
                    Debug.WriteLine($"No Tle with Name containing '{textbox.Text}' found.");
                }

                sat_list.ItemsSource = null;
                sat_list.ItemsSource = SatList;
            }
            finally
            {
                LoadingRing.IsActive = false;
                LoadingRing.Visibility = Visibility.Collapsed;
            }
        }

        private async void Apply_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = SatList.Where(item => item.IsSelected).ToList();

            // 从本地文件获取已有的数据
            var localData = await new SimpleSatData("",0).GetSatDataLocalAsync();

            // 判断是否有更改
            bool isChanged = IsDataChanged(localData, selectedItems);

            if (isChanged)
            {
                Debug.WriteLine("Data has changed, updating local file...");
                SimpleSatData.SetSatDataLocalAsync(selectedItems);
                Debug.WriteLine("Data saved successfully.");
            }
            else
            {
                Debug.WriteLine("No changes detected, no need to update local file.");
            }

            Frame.Navigate(typeof(SettingPage));
        }
        private bool IsDataChanged(List<SimpleSatData> localData, List<SimpleSatData> currentData)
        {
            // 如果本地数据和当前选中数据的数量不同，肯定有更改
            if (localData.Count != currentData.Count)
                return true;

            // 按照 Name 和 NoradNum 对比每一个对象
            foreach (var item in currentData)
            {
                var matchingItem = localData.FirstOrDefault(data => data.Name == item.Name && data.NoradNum == item.NoradNum);

                // 如果找不到匹配项，或者选中状态不同，说明有更改
                if (matchingItem == null || matchingItem.IsSelected != item.IsSelected)
                    return true;
            }

            // 所有项目都匹配，说明没有更改
            return false;
        }

    }
}
