using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using AutoSearch.Models;
using AutoSearch.Tools;
using Newtonsoft.Json;

namespace AutoSearch
{
    public class AutoSearch
    {
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);
        
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02; // Clique esquerdo pressionado
        private const uint MOUSEEVENTF_LEFTUP = 0x04;   // Clique esquerdo liberado
        
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        static void Main(string[] args)
        {
            defineConsoletitle();
            printFL();
            

            var keyTools = new KeyTools();
            var clipboard = new ClipboardHelper();
            Random rnd = new Random();

            clipboard.SetTextClipboard("testClipboard");

            Console.WriteLine("Choose a list:");
            Console.WriteLine("1 Music (default)");
            Console.WriteLine("2 Pokemon");
            Console.WriteLine("3 City");
            
            var listNumber = Console.ReadLine();
            var listName = "Music";
                
            switch (listNumber)
            {
                case "1":
                    listName = "Music";
                    break;
                case "2":
                    listName = "Pokemon";
                    break;
                case "3":
                    listName = "City";
                    break;
            }

            Console.WriteLine("How many searches do you want to do?");
            var numbersOfSearchesString = Console.ReadLine();
            int.TryParse(numbersOfSearchesString, out int numbersOfSearchesInt);
            
            var fileAddress = $"Lists\\v1\\{listName}.json";
            var jsonFile = File.ReadAllText(fileAddress);
            var listOfSearch = JsonConvert.DeserializeObject<ListOfSearch>(jsonFile);

            for (int i = 0; i < numbersOfSearchesInt; i++)
            {
                var mousePositionX = 225;
                var mousePositiony = 130;

                //for (int j = 0; j < 30; j++)
                //{
                //    //for tests
                //    var selectedValue2 = DrawName(listOfSearch);
                //}

                var selectedValue = DrawName(listOfSearch);

                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] " +
                                  $"Search {i+1} of {numbersOfSearchesString} " +
                                  $"- {listName}: {selectedValue}");
                
                clipboard.SetTextClipboard(selectedValue);
                
                MoveMouse(mousePositionX, mousePositiony);
                System.Threading.Thread.Sleep(500);
                
                MouseClick(mousePositionX, mousePositiony);
                System.Threading.Thread.Sleep(200);

                keyTools.SendCtrlA();
                keyTools.SendCtrlV();
                keyTools.SendEnter();

                //mousePositionX = 225;
                //mousePositiony = 180;
                //System.Threading.Thread.Sleep(1000*sleepInSeconds);
                //MoveMouse(mousePositionX, mousePositiony);

                //System.Threading.Thread.Sleep(1000);
                //MouseClick(mousePositionX, mousePositiony);

                System.Threading.Thread.Sleep(4000);
            }
            
            
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
            {
                return DrawName(listOfSearch);
            }

            return selectedValue;
        }

        private static string GetProjectVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version.ToString();
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

