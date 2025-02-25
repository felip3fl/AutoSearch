using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AutoSearch.Models;
using AutoSearch.Tools;
using LocalFile;
using LocalFile.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AutoSearch
{
    public class AutoSearch
    {
        static List<int> excludedNumbers = new();

        static JsonLocalFile localFile   = new();
        static Terminal terminal         = new();
        static KeyTools keyTools         = new();
        static ClipboardHelper clipboard = new();
        static MouseTools mouseTools     = new();
        static Watch watch = new();

        static void Main(string[] args)
        {
            terminal.DefineConsoletitle(Assembly.GetExecutingAssembly().GetName().Name);
            terminal.PrintFL();
            LoadConfig();

            clipboard.SetTextClipboard("testClipboard");

            var files = localFile.GetListFileName("Lists\\v1");

            Console.WriteLine("Choose a list:");
            ShowListWithDetails(files);

            var listNumber = Int32.Parse(Console.ReadLine());
            var listName = GetRecordById(files, listNumber);

            Console.WriteLine("How many searches do you want to do?");
            var numbersOfSearchesString = Console.ReadLine();

            int.TryParse(numbersOfSearchesString, out int numbersOfSearchesInt);
            
            var jsonFile = File.ReadAllText(listName.Path);
            var listOfSearch = JsonConvert.DeserializeObject<ListOfSearch>(value: jsonFile);

            Console.Clear();
            terminal.PrintFL();

            watch.Start();

            for (int i = 0; i < numbersOfSearchesInt; i++)
            {
                var selectedValue = DrawName(listOfSearch);

                PrintDateTime();
                Console.WriteLine($"{listName.Name} {i + 1}/{numbersOfSearchesString}: " +
                                        $"{selectedValue}");

                Search(selectedValue);
            }

            watch.Stop();
            Console.Write($"\nFINISH - Total time: {watch.getTotalTime()}");

            OpenPointPage();
            Console.ReadLine();
        }

        private static void Search(string selectedValue)
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

            System.Threading.Thread.Sleep(4000);
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
            var connectionString = config.GetConnectionString("ConnectionString");
        }

        public static bool ContainsNonAlphabeticalCharacters(string input)
        {
            var isMatch = !Regex.IsMatch(input, @"^[a-zA-Z"",.()!?'\-\s]*$");
            return isMatch;
        }

        private static string DrawName(ListOfSearch listOfSearch)
        {
            Random rnd = new Random();

            int musicIndex = rnd.Next(1, listOfSearch.Name.Count());

            if (CheckExcludedNumbers(musicIndex))
                return DrawName(listOfSearch);

            AddExcludedNumbers(musicIndex);

            var selectedValue = listOfSearch.Name[musicIndex];

            if (ContainsNonAlphabeticalCharacters(selectedValue))
                return DrawName(listOfSearch);
            
            return selectedValue;
        }

        private static bool CheckExcludedNumbers(int number)
        {
            if (excludedNumbers.Contains(number))
            {
                return true;
            }
            return false;
        }

        private static void AddExcludedNumbers(int numberToExclude)
        {
            excludedNumbers.Add(numberToExclude);
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

