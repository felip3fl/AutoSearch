using AutoSearch.Tools;
using LocalFile;
using LocalFile.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Search;
using Search.Models;
using System.Diagnostics;
using System.Reflection;

namespace AutoSearch
{
    public class AutoSearch
    {
        static JsonLocalFile localFile   = new();
        static Terminal terminal         = new();
        static KeyTools keyTools         = new();
        static ClipboardHelper clipboard = new();
        static MouseTools mouseTools     = new();
        static AutomateSearch automateSearch = new();

        static Watch watch = new();

        static void Main(string[] args)
        {
            terminal.DefineConsoletitle(Assembly.GetExecutingAssembly().GetName().Name);
            terminal.PrintFL();

            clipboard.SetTextClipboard("testClipboard");

            var files = localFile.GetListFileName("Lists\\v1");

            Console.WriteLine("Choose a list:");
            ShowListWithDetails(files);

            var listNumber = Int32.Parse(Console.ReadLine());
            var listName = GetRecordById(files, listNumber);

            Console.WriteLine("\nHow many searches do you want to do?");
            var numbersOfSearchesString = Console.ReadLine();

            Console.WriteLine("\nWhat type of search do you want?");
            Console.WriteLine("1 Normal (default)");
            Console.WriteLine("2 Search and update page");
            var typeSearch = Console.ReadLine();

            Console.WriteLine("\nTime interval (seconds)? ");
            var timeInterval = Int32.Parse(Console.ReadLine());

            int.TryParse(numbersOfSearchesString, out int numbersOfSearchesInt);
            
            var jsonFile = File.ReadAllText(listName.Path);
            var listOfSearch = JsonConvert.DeserializeObject<ListOfSearch>(value: jsonFile);

            Console.Clear();
            terminal.PrintFL();

            watch.Start();

            for (int i = 0; i < numbersOfSearchesInt; i++)
            {
                var selectedValue = automateSearch.DrawName(listOfSearch);

                PrintDateTime();
                Console.WriteLine($"{listName.Name} {i + 1}/{numbersOfSearchesString}: " +
                                        $"{selectedValue}");

                if(typeSearch == "1")
                    Search(selectedValue, timeInterval);
                if (typeSearch == "2")
                    SearchAndUpdatePage(selectedValue, timeInterval);
            }

            watch.Stop();
            Console.Write($"\nFINISH - Total time: {watch.getTotalTime()}");

            OpenPointPage();
            Console.ReadLine();
        }

        private static void SearchAndUpdatePage(string selectedValue, int inverval)
        {
            var mousePositionX = 225;
            var mousePositiony = 130;

            terminal.SetFocus();
            clipboard.SetTextClipboard(selectedValue);

            mouseTools.MoveMouse(mousePositionX, mousePositiony, 500);
            mouseTools.MouseClick(mousePositionX, mousePositiony, 200);

            keyTools.SendCtrlA();
            keyTools.SendCtrlV();
            keyTools.SendEnter();

            keyTools.SendF5(3000, 1000*inverval);
        }

        private static void Search(string selectedValue, int inverval)
        {
            var mousePositionX = 225;
            var mousePositiony = 130;

            terminal.SetFocus();
            clipboard.SetTextClipboard(selectedValue);

            mouseTools.MoveMouse(mousePositionX, mousePositiony, 500);
            mouseTools.MouseClick(mousePositionX, mousePositiony, 200);

            keyTools.SendCtrlA();
            keyTools.SendCtrlV();
            keyTools.SendEnter();

            Thread.Sleep(inverval * 1000);
        }

        private static Record GetRecordById(List<Record> files, int Id)
        {
            
            var listName = files.Where(x => x.Id == Id).FirstOrDefault();
            return listName;
        }

        private static void PrintFileTotalSize(Record fileId)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" {fileId.Size}");
            Console.ResetColor();
        }

        private static void ShowListWithDetails(List<Record> files)
        {
            foreach (var item in files)
            {
                Console.Write($"{item.Id} {item.Name}");
                PrintFileTotalSize(item);
                Console.Write(Environment.NewLine);
            }
        } 

        private static void PrintDateTime()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now.ToLongTimeString()}] ");
            Console.ResetColor();
        }

        public static void LoadConfig()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");

            var config = configuration.Build();
            var oi = SettingsConfig.GetSetting("MousePosition");

            var connectionString = config.GetConnectionString("MousePosition");
        }


        private static void OpenWebSite(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private static void OpenPointPage()
        {
            OpenWebSite("https://rewards.bing.com/redeem/pointsbreakdown");
        }

    }
}

