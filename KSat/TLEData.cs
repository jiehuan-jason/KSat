using SGPdotNET.CoordinateSystem;
using SGPdotNET.Observation;
using SGPdotNET.TLE;
using SGPdotNET.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace KSat
{
    class TLEData
    {
        static public async Task<String> getLocalTLEString()
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync("tle.txt");
            String tle = await FileIO.ReadTextAsync(file);
            return tle;
        }

        static public async Task<List<Tle>> getLocalTLEs()
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.GetFileAsync("tle.txt");
            var TLELines = await FileIO.ReadLinesAsync(file);
            List<Tle> tle_list = new List<Tle>();
            for (int i = 0; i < TLELines.Count; i += 3)
            {
                if (i + 2 < TLELines.Count)
                {
                    string line1 = TLELines[i];
                    string line2 = TLELines[i + 1];
                    string line3 = TLELines[i + 2];
                    if (!line1.StartsWith("STARLINK"))
                    {
                        var tle = new Tle(line1, line2, line3);
                        tle_list.Add(tle);
                    }
                }
            }
            return tle_list;
        }

        static public async Task<int> getSatsCount()
        {
            List<Tle> tles = await getLocalTLEs();
            return tles.Count;
        }

        static public async Task<Boolean> GetTLEDataFromWeb()
        {
            //var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            // Remote URL
            await FetchAndSaveToFileAsync("https://celestrak.org/NORAD/elements/gp.php?GROUP=active&FORMAT=tle");
            return true;
            /*// Create a provider whose cache is renewed every 12 hours
            var provider = new RemoteTleProvider(true, url);

            var sat_count = provider.GetTles();

            Debug.WriteLine(sat_count.Count);

            // Get every TLE
            //var tles = provider.GetTles();

            // Alternatively get a specific satellite's TLE
            var isstle = provider.GetTle(sat_num);
            var sat = new Satellite(isstle);          

            // 定义地面站位置（假设在北京） 
            var location = new GeodeticCoordinate(Angle.FromDegrees(29.9), Angle.FromDegrees(121.5), 0);
            var groundStation = new GroundStation(location);

            var observations = groundStation.Observe(sat, DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromHours(24), TimeSpan.FromSeconds(10));
            var topo = groundStation.Observe(sat, DateTime.UtcNow + TimeSpan.FromHours(24));      // 打印过境信息 
            List<SatPassingData> sat_passing = new List<SatPassingData>();
            foreach (var observation in observations) 
            {
                
                Debug.WriteLine($"最高仰角: {observation.MaxElevation} 度");
                Debug.WriteLine($"入境时间: {observation.Start}");
                Debug.WriteLine($"出境时间: {observation.End}");
                var start_topo = groundStation.Observe(sat, observation.Start);
                var end_topo = groundStation.Observe(sat, observation.End);
                var max_topo = groundStation.Observe(sat, observation.MaxElevationTime);
                Debug.WriteLine($"入境方位角: {start_topo.Azimuth} 度");
                Debug.WriteLine($"出境方位角: {end_topo.Azimuth} 度");
                Debug.WriteLine($"最高仰角方位角: {max_topo.Azimuth} 度");
                sat_passing.Add(new SatPassingData(start_topo.Azimuth, end_topo.Azimuth, observation.MaxElevation, max_topo.Azimuth, observation.Start, observation.End));
            }
            return sat_passing;
            //var observations = groundStation.Observe(sat, DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromHours(24), TimeSpan.FromSeconds(10));
            //Debug.WriteLine(issTle);*/
        }
        public static async Task FetchAndSaveToFileAsync(string url)
        {
            try
            {
                // 创建 HttpClient 实例
                using (HttpClient httpClient = new HttpClient())
                {
                    // 发送 GET 请求获取网页内容
                    string content = await httpClient.GetStringAsync(url);

                    // 获取应用的本地存储文件夹
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                    //Debug.WriteLine(localFolder.DisplayName);

                    // 创建或获取文件 tle.txt
                    StorageFile tleFile = await localFolder.CreateFileAsync("tle.txt", CreationCollisionOption.ReplaceExisting);

                    // 将网页内容写入文件
                    await FileIO.WriteTextAsync(tleFile, content);

                    // 显示保存成功的消息
                    System.Diagnostics.Debug.WriteLine("内容已成功保存到 tle.txt 文件中！");
                }
            }
            catch (Exception ex)
            {
                // 错误处理
                System.Diagnostics.Debug.WriteLine($"发生错误：{ex.Message}");
            }
        }
    }

}
