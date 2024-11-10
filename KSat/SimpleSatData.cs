using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace KSat
{
    class SimpleSatData
    {
        public String name;
        public int norad_num;
        public SimpleSatData(String name, int norad_num)
        {
            this.name = name;
            this.norad_num = norad_num;
        }
        public async Task<List<SimpleSatData>> GetSatDataLocal()
        {
            string fileName = "select.txt";
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var list = new List<SimpleSatData>();
            try
            {
                // 尝试获取文件
                StorageFile file = await localFolder.GetFileAsync(fileName);

                // 文件存在，读取内容
                var content = await FileIO.ReadLinesAsync(file);
                for(int i = 0; i < content.Count; i += 2)
                {
                    list.Add(new SimpleSatData(content[i], int.Parse(content[i + 1])));
                }
                return list;
            }
            catch (System.IO.FileNotFoundException)
            {
                // 文件不存在
                StorageFile newFile = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                return list;
            }

        }
        public async void SetSatDataLocal(List<SimpleSatData> list)
        {
            string fileName = "select.txt";
            List<string> lines = new List<string>();
            foreach (var data in list)
            {
                lines.Add(data.name); // 第一行：Name
                lines.Add(data.norad_num.ToString());   // 第二行：ID
            }
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteLinesAsync(file, lines);

        }
        public async Task<Boolean> isSatSelected(string targetName)
        {
            List<SimpleSatData> dataList = await GetSatDataLocal();
            // 使用 LINQ 查询找到 Name 匹配的对象
            if (dataList.FirstOrDefault(item => item.name == targetName) == null) return false;
            else return true;
        }
    }
}
