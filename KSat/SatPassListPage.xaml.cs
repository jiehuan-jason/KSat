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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace KSat
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SatPassListPage : Page
    {
        public SimpleSatData sat;
        public SatPassListPage()
        {
            this.InitializeComponent();
            
        }

        private void Sat_list_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private async void showPassingList()
        {
            var passing_list = new PassDataList(sat, new GeodeticCoordinate(Angle.FromDegrees(29.9), Angle.FromDegrees(121.5), 0));
            await passing_list.getPassDataList(sat.NoradNum);
            sat_list.ItemsSource = passing_list.pass_list;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // 获取导航传递的数据
            if (e.Parameter is SimpleSatData satData)
            {
                sat = satData;
                showPassingList();
                // 显示数据或使用
                Title.Content = satData.Name;
            }
        }
    }
}
