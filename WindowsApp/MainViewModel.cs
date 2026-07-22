using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFile;
using LocalFile.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.UI.Xaml.Controls;
using Search;
using Search.Models;
using Search.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using WindowsApp.Command;
using WindowsApp.Model;

namespace WindowsApp
{
    public partial class MainViewModel : ObservableObject
    {
        public Record SelectedSearchListOption { get; set; } = new();
        public EmailSettings emailSettings;
        static JsonLocalFile localFile = new();
        static AutomateSearch automateSearch = new();
        static EmailService _emailService = new(new());
        public int searchDone = 0;
        private CmdExecutor _cmdExecutor = new();
        private AutomateSearch _superAutomateSearch = new();
        private readonly SynchronizationContext _uiContext;
        public List<string> logs = new();

        [ObservableProperty]
        public partial List<string> loog { get; set; } = new() {"Os logs serão exibidos aqui"};

        [ObservableProperty]
        public partial int HowLongTime { get; set; } = 60;

        [ObservableProperty]
        public partial int HowManySeacrh { get; set; } = 30;

        [ObservableProperty]
        public partial string Status { get; set; } = "Iniciar";

        [ObservableProperty]
        public partial bool IsRunning { get; set; } = false;

        [ObservableProperty]
        public partial bool TurnOffComputer { get; set; } = true;

        [ObservableProperty]
        public List<Record> searchListOption = localFile.GetListFileName("Lists\\v1");



        public  FrontText frontText { get; set; } = new()
        {
            ProjectName = "FLex auto search",
            ProjectVersion = $"Versão {GetAssemblyVersion()}",
            HowManySearch = "Quantas pesquisas",
            HowManySearchSubText = "Quantas pesquisas",
            HowLong = "Tempo em segundos",
            HowLongSubText = "Tempo em segundos",
            ListOfSearchOption = "Lista de pesquisa:",
            TurnOfComputer = new OptionText() { mainDescription = "Desligar computator", turnOff = "Não", turnOn = "Sim" },
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

        public void UpdateEstimatedTime(int searchQuantity, int howLongTime)
        {
            var totalTimeInSeconds = searchQuantity * howLongTime;
            var duration = TimeSpan.FromSeconds(totalTimeInSeconds);
            frontText.HowManySearchSubText = $"Duração total: {duration.ToString(@"hh\hmm\m")}";
            _superAutomateSearch.SetTimeInterval(howLongTime);
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

        public async void UpdateRunningStatus(bool isRunning)
        {
            if (isRunning)
            {
                addValue("Iniciando pesquisa em 5 segundos");
                updateStatus("Parar pesquisa");
                return;
            }

            updateStatus("Iniciar");
            addValue("Pesquisa interrompida");
        }

        private void updateStatus(string value)
        {
            if (SynchronizationContext.Current == _uiContext)
                Status = value;
            else
                _uiContext.Post(_ => Status = value, null);
        }

        private async void ChangeRunningStatus()
        {
            IsRunning = !IsRunning;
            _superAutomateSearch.UpdateRunningStatus(IsRunning);

            UpdateRunningStatus(IsRunning);
        }

        public MainViewModel()
        {   
            _uiContext = SynchronizationContext.Current!;

            _superAutomateSearch.processCompleted += CompleteProcess;
            _superAutomateSearch.errorTask += ErrorProcess;
            _superAutomateSearch.runningSearch += UpdateLog;
            _superAutomateSearch.SetTimeInterval(HowLongTime);

            AddCommand = new DelegateCommand(StartSearch);
            SelectedSearchListOption = LoadingSelectedList();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<MainViewModel>()
                .Build();

            emailSettings = config.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
            _emailService = new EmailService(emailSettings);
        }

        public DelegateCommand AddCommand { get; set; }

        private void UpdateLog(object? sender, string e)
        {
            searchDone++;
            var newValue = $"[{DateTime.Now.ToLongTimeString()}] {SelectedSearchListOption.Name} {searchDone}/{HowManySeacrh} - {e}";
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

        private async void CompleteProcess(object sender, EventArgs e)
        {
            ChangeRunningStatus();

            await _emailService.SendEmailAsync("felip3.fl@gmail.com", "FLex auto search - Processo Concluído", "Processo concluído com sucesso.");

            var newValue = $"[{DateTime.Now.ToLongTimeString()}] Pesquisa finalizada";
            addValue(newValue);

            if (TurnOffComputer) 
                _cmdExecutor.ShutDownComputer(60);
        }

        private async void ErrorProcess(object sender, EventArgs e)
        {
            ChangeRunningStatus();
            await _emailService.SendEmailAsync("felip3.fl@gmail.com", "FLex auto search - Falha", "Ocorreu um erro durante o processo.");

            if (TurnOffComputer)
                _cmdExecutor.ShutDownComputer(120);
        }
        
        public async Task StartSearch(List<string> listOfSearchText, int timeInterval)
        {
            ChangeRunningStatus();

            Task.Run(async () => _superAutomateSearch.SearchAsync(listOfSearchText));
                
        }

        private void StartSearch(object? parameter)
        {

            try
            {
                ChangeRunningStatus();

                if (!IsRunning)
                    return;
                

                var listNumber = SelectedSearchListOption;

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

                Task.Run(async () => _superAutomateSearch.SearchAsync(listOfSearchText));

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