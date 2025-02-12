using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AutoSearch.Models;
using AutoSearch.Tools;
using LocalFile;
using LocalFile.Models;
using Newtonsoft.Json;

namespace AutoSearch
{
    public class AutoSearch
    {
        static List<int> excludedNumbers = new();
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
        
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02; // Clique esquerdo pressionado
        private const uint MOUSEEVENTF_LEFTUP = 0x04;   // Clique esquerdo liberado
        
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        static void Main(string[] args)
        {
            var localFile = new LocalFile.LocalFile();
            var terminal = new Terminal();
            var files = localFile.GetListFileName("Lists\\v1");

            defineConsoletitle();
            printFL();
            

            var keyTools = new KeyTools();
            var clipboard = new ClipboardHelper();
            Random rnd = new Random();

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
            printFL();

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
                
                MoveMouse(mousePositionX, mousePositiony);
                System.Threading.Thread.Sleep(500);
                
                MouseClick(mousePositionX, mousePositiony);
                System.Threading.Thread.Sleep(200);

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

        public static bool ContainsNonAlphabeticalCharacters(string input)
        {
            var isMatch = !Regex.IsMatch(input, @"^[a-zA-Z"",.()!?'\-\s]*$");
            return isMatch;
        }

        private static string DrawName(ListOfSearch listOfSearch)
        {
            Random rnd = new Random();

            int musicIndex = rnd.Next(1, listOfSearch.Name.Count());

            var selectedValue = listOfSearch.Name[musicIndex];

            if (ContainsNonAlphabeticalCharacters(selectedValue))
                return DrawName(listOfSearch);

            if (CheckExcludedNumbers(musicIndex))
                return DrawName(listOfSearch);

            AddExcludedNumbers(musicIndex);
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

        private static string GetProjectVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version.ToString();
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

        private static void printFL()
        {
            var projectVersion = "v" + GetProjectVersion();
            var projectVersionLength = projectVersion.Length;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("____________________________"); 
            Console.WriteLine("\\  ________________________ \\");
            Console.WriteLine(" \\ \\        ____   __      \\ \\");
            Console.WriteLine("  \\ \\      / ___\\ /\\ \\      \\ \\");
            Console.WriteLine("   \\ \\    /\\ \\__/ \\ \\ \\      \\ \\");
            Console.WriteLine("    \\ \\   \\ \\  __\\ \\ \\ \\      \\ \\");
            Console.WriteLine("     \\ \\   \\ \\ \\_/  \\ \\ \\      \\ \\");
            Console.WriteLine("      \\ \\   \\ \\ \\    \\ \\ \\___   \\ \\");
            Console.WriteLine("       \\ \\   \\ \\_\\    \\ \\____\\   \\ \\");
            Console.WriteLine("        \\ \\   \\/_/     \\/____/    \\ \\");
            Console.WriteLine("         \\ \\_______________________\\ \\");


            Console.WriteLine($"          \\_______________{projectVersion}__\\");

            Console.WriteLine(@"");
            Console.ResetColor();
        }

        private static void defineConsoletitle()
        {
            var title = Assembly.GetExecutingAssembly().GetName().Name;
            Console.Title = title;
        }

        private static void Countdown(int timeInSeconds)
        {
            Console.Write("Starting in ");
            for (int i = 0; i < timeInSeconds; i++)
            {
                Console.Write((timeInSeconds- i) + " ");
                System.Threading.Thread.Sleep(1000);
            }
            Console.WriteLine(" ");
        }

        private static void MoveMouse(int positionX, int positionY)
        {
            SetCursorPos(positionX, positionY);
        }
        
        private static void MouseClick(int positionX, int positionY)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, positionX, positionY, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, positionX, positionY, 0, UIntPtr.Zero); 
        }

    }
}

