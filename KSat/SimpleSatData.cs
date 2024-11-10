using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace KSat
{
    public class SimpleSatData : INotifyPropertyChanged
    {
        private string _name;
        private int _noradNum;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public int NoradNum
        {
            get => _noradNum;
            set
            {
                _noradNum = value;
                OnPropertyChanged(nameof(NoradNum));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public SimpleSatData(string name, int noradNum)
        {
            Name = name;
            NoradNum = noradNum;
        }

        public async Task InitializeSelectionStatusAsync()
        {
            IsSelected = await IsSatSelectedAsync(Name);
        }

        public async Task<List<SimpleSatData>> GetSatDataLocalAsync()
        {
            string fileName = "select.txt";
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var list = new List<SimpleSatData>();

            try
            {
                // 尝试获取文件
                StorageFile file = await localFolder.GetFileAsync(fileName);
                var content = await FileIO.ReadLinesAsync(file);

                for (int i = 0; i < content.Count; i += 2)
                {
                    string name = content[i];
                    int noradNum = int.Parse(content[i + 1]);
                    var data = new SimpleSatData(name, noradNum);
                    //await data.InitializeSelectionStatusAsync();
                    list.Add(data);
                }

                return list;
            }
            catch (System.IO.FileNotFoundException)
            {
                // 文件不存在，创建新文件
                await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                return list;
            }
        }

        static public async void SetSatDataLocalAsync(List<SimpleSatData> list)
        {
            string fileName = "select.txt";
            List<string> lines = new List<string>();

            foreach (var data in list)
            {
                lines.Add(data.Name);
                lines.Add(data.NoradNum.ToString());
            }

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteLinesAsync(file, lines);
        }

        public async Task<bool> IsSatSelectedAsync(string targetName)
        {
            List<SimpleSatData> dataList = await GetSatDataLocalAsync();
            return dataList.Any(item => item.Name == targetName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
