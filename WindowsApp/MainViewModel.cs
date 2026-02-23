using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFile;
using LocalFile.Models;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json.Linq;
using Search;
using Search.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using WindowsApp.Model;

namespace WindowsApp
{
    public partial class MainViewModel : ObservableObject
    {
        static JsonLocalFile localFile = new();
        
        public ObservableCollection<string> logs = new();

        [ObservableProperty]
        public partial string Status { get; set; } = "Start";

        public  FrontText frontText { get; set; } = new()
        {
            ProjectName = "FLex auto search",
            ProjectVersion = "Versão 1.26.02.21",
            HowManySearch = "Quantas pesquisas",
            HowLong = "Tempo em segundos",
            ListOfSearchOption = "Lista de pesquisa:",
            TurnOfComputer = new OptionText() { mainDescription = "Desligar pós pesquisa", turnOff = "Manter ligado", turnOn = "Desligar" },
            UpdateThePage = new OptionText() { mainDescription = "Atualizar página", turnOff = "Apenas pesquisar", turnOn = "Atualizar" },
            MainButton = "Começar",
            Log = "Log",
        };
        
        [ObservableProperty]
        public partial bool IsRunning { get; set; } = false;

        [ObservableProperty]
        public List<Record> searchListOption = localFile.GetListFileName("Lists\\v1");

        
        public int searchDone = 0;

        private CmdExecutor _cmdExecutor = new();
        private AutomateSearch _superAutomateSearch = new();

        private readonly SynchronizationContext _uiContext;

        [RelayCommand]
        public void UpdateStatus()
        {
            Status = "Stop running";
        }

        public void UpdateRunningStatus()
        {
            Status = "Start";

            if (IsRunning)
                Status = "Stop search";
        }

        public MainViewModel()
        {   
            _uiContext = SynchronizationContext.Current!;
            UpdateRunningStatus();

            _superAutomateSearch.processCompleted += CompleteProcess;
            _superAutomateSearch.runningSearch += UpdateLog;
        }

        private void UpdateLog(object? sender, string e)
        {
            searchDone++;
            var newValue = $"[{DateTime.Now.ToLongTimeString()}] Lista {searchDone}/{30} - {e}";
            addValue(newValue);
        }

        private void addValue(string value)
        {
            if (SynchronizationContext.Current == _uiContext)
                logs.Add(value);
            else
                _uiContext.Post(_ => logs.Insert(0, value), null);
        }

        private void CompleteProcess(object sender, EventArgs e)
        {
            _cmdExecutor.ShutDownComputer(20);
        }
        
        public async Task StartSearch(List<string> listOfSearchText, int timeInterval)
        {
            frontText.ProjectName = "aasdjoialskd";

            IsRunning = !IsRunning;
            UpdateRunningStatus();

            if (!IsRunning)
            {
                _cmdExecutor.StopShutDownComputer();
                return;
            }

            Task.Run(async () => _superAutomateSearch.SearchAsync(listOfSearchText, timeInterval));
                
        }

    }
}