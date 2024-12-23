using System.Runtime.InteropServices;
using AutoSearch.Models;
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
            var fileAddress = "Lists\\v1\\List.json";
            var jsonFile = File.ReadAllText(fileAddress);
            var listOfSearch = JsonConvert.DeserializeObject<ListOfSearch>(jsonFile);
            
            var numbersOfSearches = listOfSearch.Name.Count();

            Countdown(3);

            for (int i = 0; i < numbersOfSearches; i++)
            {
                Random rnd = new Random();
                int musicIndex  = rnd.Next(1, numbersOfSearches);

                var selectedValue = listOfSearch.Name[musicIndex];
                SetClipboard(selectedValue);

                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Search {i+1} of {numbersOfSearches}" +
                                  $" - Music: {selectedValue}");
                
                int sleepInSeconds  = rnd.Next(1, 3);
                
                System.Threading.Thread.Sleep(1001*sleepInSeconds);
                MoveMouse(225, 130);
                
                System.Threading.Thread.Sleep(1002*sleepInSeconds);
                MouseClick(225, 180);

                System.Threading.Thread.Sleep(1030*sleepInSeconds);
                SendKeys.SendWait("^a");
            
                System.Threading.Thread.Sleep(1040*sleepInSeconds);
                SendKeys.SendWait("^v");
                
                System.Threading.Thread.Sleep(1050*sleepInSeconds);
                MoveMouse(225, 180);
                
                System.Threading.Thread.Sleep(1060*sleepInSeconds);
                MouseClick(225, 180);
            
                // System.Threading.Thread.Sleep(1000);
                // SendKeys.SendWait("{ENTER}");
            }
            
            System.Threading.Thread.Sleep(2500);
            
            // SendKeys.SendWait("{DELETE}");
            // System.Threading.Thread.Sleep(1000);
            
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
            try
            {
                var thread = new Thread(() => Clipboard.SetText(text));
                thread.SetApartmentState(ApartmentState.STA); 
                thread.Start();
                thread.Join();              
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: SetClipboard");
                System.Threading.Thread.Sleep(1000);
                SetClipboard(text);
            }

        }
        
    }
}

