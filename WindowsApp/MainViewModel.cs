using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace WindowsApp
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string Status { get; set; } = "Start";

        [RelayCommand]
        public void UpdateStatus()
        {
            Status = "Stop running";
        }
    }
}
