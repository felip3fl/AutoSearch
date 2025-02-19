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

        static void Main(string[] args)
        {
            var localFile  = new JsonLocalFile();
            var terminal   = new Terminal();
            var keyTools   = new KeyTools();
            var clipboard  = new ClipboardHelper();
            var mouseTools = new MouseTools();

            var files = localFile.GetListFileName("Lists\\v1");

            terminal.DefineConsoletitle(Assembly.GetExecutingAssembly().GetName().Name);
            terminal.PrintFL();
            loadConfig();

            
            Random rnd = new Random();

            Console.SetWindowSize(600,600);

            clipboard.SetTextClipboard("testClipboard");

            Console.WriteLine("Choose a list:");
            foreach (var item in files)
            {
                Console.Write($"{item.Id} {item.Name}");
                PrintFileTotalSize(item);
                Console.Write(Environment.NewLine);
            }

            
            var listNumber = Int32.Parse(Console.ReadLine());
            
            var listName = files.Where(x => x.Id == listNumber).FirstOrDefault();
            

            Console.WriteLine("How many searches do you want to do?");
            var numbersOfSearchesString = Console.ReadLine();
            int.TryParse(numbersOfSearchesString, out int numbersOfSearchesInt);
            
            var jsonFile = File.ReadAllText(listName.Path);
            var listOfSearch = JsonConvert.DeserializeObject<ListOfSearch>(value: jsonFile);

            Console.Clear();
            terminal.PrintFL();

            var watch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < numbersOfSearchesInt; i++)
            {
                var mousePositionX = 225;
                var mousePositiony = 130;

                var selectedValue = DrawName(listOfSearch);

                PrintDateTime();
                Console.WriteLine($"{listName.Name} {i+1}/{numbersOfSearchesString}: " +
                                        $"{selectedValue}");

                terminal.SetFocus();
                clipboard.SetTextClipboard(selectedValue);

                mouseTools.MoveMouse(mousePositionX, mousePositiony, 500);
                mouseTools.MouseClick(mousePositionX, mousePositiony, 200);

                keyTools.SendCtrlA();
                keyTools.SendCtrlV();
                keyTools.SendEnter();

                System.Threading.Thread.Sleep(4000);
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            TimeSpan t = TimeSpan.FromMilliseconds(elapsedMs);
            string answer = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);

            OpenPointPage();
            Console.Write($"\nFINISH - Total time: {answer}");
            Console.ReadLine();
        }

        private static void PrintFileTotalSize(Record fileId)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($" {fileId.Size}");
            Console.ResetColor();
        }


        private static void PrintDateTime()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now.ToLongTimeString()}] ");
            Console.ResetColor();
        }

        public static void loadConfig()
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

