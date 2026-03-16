using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFile;
using LocalFile.Models;
using Microsoft.UI.Xaml.Controls;
using Search;
using Search.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WindowsApp.Command;
using WindowsApp.Model;

namespace WindowsApp
{
    public partial class MainViewModel : ObservableObject
    {


        static JsonLocalFile localFile = new();
        static AutomateSearch automateSearch = new();

        [ObservableProperty]
        public partial List<string> loog { get; set; } = new() {"Os logs serão exibidos aqui"};

        public List<string> logs = new();


        [ObservableProperty]
        public partial int HowLongTime { get; set; } = 60;



        public Record SelectedSearchListOption { get; set; } = new ();

        [ObservableProperty]
        public partial int HowManySeacrh { get; set; } = 30;

        [ObservableProperty]
        public partial string Status { get; set; } = "Start";

        [ObservableProperty]
        public partial bool IsRunning { get; set; } = false;

        [ObservableProperty]
        public partial bool TurnOffComputer { get; set; } = true;

        [ObservableProperty]
        public List<Record> searchListOption = localFile.GetListFileName("Lists\\v1");


        public int searchDone = 0;

        private CmdExecutor _cmdExecutor = new();
        private AutomateSearch _superAutomateSearch = new();

        private readonly SynchronizationContext _uiContext;

        public  FrontText frontText { get; set; } = new()
        {
            ProjectName = "FLex auto search",
            ProjectVersion = $"Versão {GetAssemblyVersion()}",
            HowManySearch = "Quantas pesquisas",
            HowManySearchSubText = "Quantas pesquisas",
            HowLong = "Tempo em segundos",
            HowLongSubText = "Tempo em segundos",
            ListOfSearchOption = "Lista de pesquisa:",
            TurnOfComputer = new OptionText() { mainDescription = "Desligar computador", turnOff = "Manter ligado", turnOn = "Desligar" },
            UpdateThePage = new OptionText() { mainDescription = "Atualizar página", turnOff = "Apenas pesquisar", turnOn = "Atualizar" },
            MainButton = "Começar",
            Log = "Log",

        };
        
        [RelayCommand]
        public void UpdateStatus()
        {
            Status = "Stop running";
        }

        public static string GetAssemblyVersion()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            if (assembly == null)
            {
                return "0.0.0.0";
            }

            var fileVersionAttr = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            if (!string.IsNullOrEmpty(fileVersionAttr?.Version))
            {
                return fileVersionAttr.Version!;
            }

            var nameVersion = assembly.GetName()?.Version?.ToString();
            return !string.IsNullOrEmpty(nameVersion) ? nameVersion! : "0.0.0.0";
        }

        public void HowManySearchSubTextUpdate(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            var searchQuantity = (int)args.NewValue;
            UpdateEstimatedTime(searchQuantity, HowLongTime);
            
        }

        public void HowLongSubTextUpdate(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            var timeInSeconds = (int)args.NewValue;
            UpdateTotalTime(timeInSeconds);
            UpdateEstimatedTime(HowManySeacrh, timeInSeconds);
        }

        public void UpdateEstimatedTime(int searchQuantity, int HowLongTime)
        {
            var totalTimeInSeconds = searchQuantity * HowLongTime;
            var duration = TimeSpan.FromSeconds(totalTimeInSeconds);
            frontText.HowManySearchSubText = $"Duração total: {duration.ToString(@"hh\hmm\m")}";
        }

        public void UpdateTotalTime(int timeInSeconds)
        {
            var duration = TimeSpan.FromMinutes(timeInSeconds);
            frontText.HowLongSubText = $"Minutos: {duration.ToString(@"hh\mmm\s")}";
        }

        private Record LoadingSelectedList()
        {
            DateTime todayDate = DateTime.Now;
            var dayOfWeek = (int)todayDate.DayOfWeek;
            return searchListOption[dayOfWeek];
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
            AddCommand = new DelegateCommand(StartSearch);
            SelectedSearchListOption = LoadingSelectedList();
        }

        public DelegateCommand AddCommand { get; set; }

        private void UpdateLog(object? sender, string e)
        {
            searchDone++;
            var newValue = $"[{DateTime.Now.ToLongTimeString()}] {SelectedSearchListOption.Name} {searchDone}/{30} - {e}";
            addValue(newValue);
        }

        private void addValue(string value)
        {
            logs.Insert(0,value);
            if (SynchronizationContext.Current == _uiContext)
                loog = new List<string>(logs);
            else
                _uiContext.Post(_ => loog = new( logs), null);
        }

        private void CompleteProcess(object sender, EventArgs e)
        {
            _cmdExecutor.ShutDownComputer(20);
        }
        
        public async Task StartSearch(List<string> listOfSearchText, int timeInterval)
        {
            addValue("Iniciando pesquisa em 5 segundos");
            IsRunning = !IsRunning;
            UpdateRunningStatus();

            Task.Run(async () => _superAutomateSearch.SearchAsync(listOfSearchText, timeInterval));
                
        }

        private void StartSearch(object? parameter)
        {

            try
            {
                var listNumber = SelectedSearchListOption;
                var oi = TurnOffComputer;
                return;

                List<string> listOfSearchText = new();

                if (listNumber == null)
                    return;

                var listName = GetRecordById(SearchListOption, listNumber.Id);

                var numbersOfSearchesString = HowManySeacrh;


                var timeInterval = HowLongTime;

                var fileContent = System.IO.File.ReadAllLines(listName.Path);

                if (fileContent == null)
                    return;

                var listFileContent = fileContent.ToList();

                for (int i = 0; i < numbersOfSearchesString; i++)
                {
                    listOfSearchText.Add(automateSearch.DrawName(listFileContent));
                }

                addValue("Iniciando pesquisa em 5 segundos");
                IsRunning = !IsRunning;
                UpdateRunningStatus();

                Task.Run(async () => _superAutomateSearch.SearchAsync(listOfSearchText, timeInterval));

            }
            catch (Exception error)
            {
                
            }
            
        }

        private static Record GetRecordById(List<Record> files, int Id)
        {
            var listName = files.Where(x => x.Id == Id).FirstOrDefault();
            return listName;
        }

    }
}