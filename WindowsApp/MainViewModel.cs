using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFile;
using LocalFile.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WindowsApp
{
    public partial class MainViewModel : ObservableObject
    {
        static JsonLocalFile localFile = new();

        [ObservableProperty]
        public partial string Status { get; set; } = "Start";

        [ObservableProperty]
        public List<Record> searchListOption = localFile.GetListFileName("Lists\\v1");

        [RelayCommand]
        public void UpdateStatus()
        {
            Status = "Stop running";
        }
    }
}
