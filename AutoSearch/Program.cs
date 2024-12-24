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
            var keyTools = new KeyTools();
        
            Console.WriteLine("Enter the name of the list: ");
            Console.WriteLine("1 - Music");
            Console.WriteLine("2 - Pokemon");
            Console.WriteLine("3 - Cities");
            
            var listNumber = Console.ReadLine();
            var listName = "";
                
            switch (listNumber)
            {
                case "1":
                    listName = "Music";
                    break;
                case "2":
                    listName = "Pokemon";
                    break;
                case "3":
                    listName = "Cities";
                    break;
                default:
                    listName = "Music";
                    break;
            }
            
            var fileAddress = $"Lists\\v1\\{listName}.json";
            var jsonFile = File.ReadAllText(fileAddress);
            var listOfSearch = JsonConvert.DeserializeObject<ListOfSearch>(jsonFile);
            
            var numbersOfSearches = 35;

            // Countdown(3);

            for (int i = 0; i < numbersOfSearches; i++)
            {
                var mousePositionX = 225;
                var mousePositiony = 130;
                
                Random rnd = new Random();
                int musicIndex  = rnd.Next(1, numbersOfSearches);

                var selectedValue = listOfSearch.Name[musicIndex];

                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Search {i+1} of {numbersOfSearches}" +
                                  $" - Text: {selectedValue}");
                
                SetClipboard(selectedValue);
                
                int sleepInSeconds  = rnd.Next(1, 3);
                
                System.Threading.Thread.Sleep(1001*sleepInSeconds);
                MoveMouse(mousePositionX, mousePositiony);
                
                System.Threading.Thread.Sleep(1002*sleepInSeconds);
                MouseClick(mousePositionX, mousePositiony);

                System.Threading.Thread.Sleep(1030*sleepInSeconds);
                keyTools.SendCtrlA();
            
                System.Threading.Thread.Sleep(1040*sleepInSeconds);
                keyTools.SendCtrlV();

                mousePositionX = 225;
                mousePositiony = 180;
                
                System.Threading.Thread.Sleep(1050*sleepInSeconds);
                MoveMouse(mousePositionX, mousePositiony);
                
                System.Threading.Thread.Sleep(1060*sleepInSeconds);
                MouseClick(mousePositionX, mousePositiony);
                
            }
            
            System.Threading.Thread.Sleep(2500);
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

        private static void SetClipboard(string text)
        {
            var clipboardThread = new Thread(() => Clipboard.SetText(text));
            clipboardThread.SetApartmentState(ApartmentState.STA); 
            clipboardThread.Start();
            clipboardThread.Join();
        }
        
    }
}

