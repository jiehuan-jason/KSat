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
        private List<string> selectedSatNames = new List<string>();  // 保存已选中的卫星名称

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
                selectedSatNames = SatList.Where(sat => sat.IsSelected).Select(sat => sat.Name).ToList();

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

            }
            finally
            {
                LoadingRing.IsActive = false;
                LoadingRing.Visibility = Visibility.Collapsed;
            }
        }

        private async void Apply_Click(object sender, RoutedEventArgs e)
        {
            // 获取显示列表和选中列表
            var listItems = SatList.ToList();
            var selectedItems = SatList.Where(item => item.IsSelected).ToList();

            // 从本地文件获取已有的数据
            var localData = await new SimpleSatData("", 0).GetSatDataLocalAsync();

            // 第一步：检测显示列表中的项目并更新本地存储列表
            for (int i = listItems.Count - 1; i >= 0; i--)
            {
                var currentItem = listItems[i];

                // 如果项目存在于本地存储列表中但未被选中，从本地存储列表中删除
                if (localData.Contains(currentItem) && !currentItem.IsSelected)
                {
                    localData.Remove(currentItem);
                    Debug.WriteLine($"Removed item from local data: {currentItem.Name}");
                }
            }

            // 第二步：检测选中列表中的项目并更新本地存储列表
            foreach (var selectedItem in selectedItems)
            {
                // 如果项目不在本地存储列表中，添加到本地存储列表
                if (!localData.Contains(selectedItem))
                {
                    localData.Add(selectedItem);
                    Debug.WriteLine($"Added item to local data: {selectedItem.Name}");
                }
            }

            // 第三步：检测是否有变化并写入本地存储
            bool isChanged = IsDataChanged(localData, await new SimpleSatData("", 0).GetSatDataLocalAsync());

            if (isChanged)
            {
                Debug.WriteLine("Data has changed, updating local file...");
                SimpleSatData.SetSatDataLocalAsync(localData);
                Debug.WriteLine("Data saved successfully.");
            }
            else
            {
                Debug.WriteLine("No changes detected, no need to update local file.");
            }

            // 跳转回设置页面
            Frame.Navigate(typeof(SettingPage));
        }


        private bool IsDataChanged(List<SimpleSatData> localData, List<SimpleSatData> updatedSelectedItems)
        {
            if (localData.Count != updatedSelectedItems.Count)
            {
                return true;
            }

            for (int i = 0; i < localData.Count; i++)
            {
                if (!localData[i].Equals(updatedSelectedItems[i]))
                {
                    return true;
                }
            }

            return false;
        }


    }
}
