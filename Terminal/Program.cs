using System.Drawing;
using System.Runtime.InteropServices;
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
            printFL();

            var keyTools = new KeyTools();
            var clipboard = new ClipboardHelper();

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

            // Countdown(3);

            for (int i = 0; i < numbersOfSearchesInt; i++)
            {
                var mousePositionX = 225;
                var mousePositiony = 130;
                
                Random rnd = new Random();
                int musicIndex  = rnd.Next(1, listOfSearch.Name.Count());

                var selectedValue = listOfSearch.Name[musicIndex];

                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] " +
                                  $"Search {i+1} of {numbersOfSearchesString} " +
                                  $"- {listName} {musicIndex}: {selectedValue}");
                
                clipboard.SetTextClipboard(selectedValue);
                
                int sleepInSeconds  = rnd.Next(1, 4);
                
                MoveMouse(mousePositionX, mousePositiony);
                
                System.Threading.Thread.Sleep(500*sleepInSeconds);
                MouseClick(mousePositionX, mousePositiony);

                System.Threading.Thread.Sleep(500*sleepInSeconds);
                keyTools.SendCtrlA();
            
                System.Threading.Thread.Sleep(500*sleepInSeconds);
                keyTools.SendCtrlV();

                mousePositionX = 225;
                mousePositiony = 180;
                
                System.Threading.Thread.Sleep(500*sleepInSeconds);
                MoveMouse(mousePositionX, mousePositiony);
                
                System.Threading.Thread.Sleep(500);
                MouseClick(mousePositionX, mousePositiony);

                System.Threading.Thread.Sleep(3000);
            }
            
            
        }

        private static void printFL()
        {
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
            Console.WriteLine("          \\___________________________\\");
            Console.WriteLine(@"");
            Console.ResetColor();
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

