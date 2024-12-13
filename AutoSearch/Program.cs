using System;
using System.Globalization;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;

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
            var fileAddress = @"C:\Users\Felipe\OneDrive\Music\This is All\zsync.spotdl";
            var jsonFile = File.ReadAllText(fileAddress);
            Regex regex = new Regex(@"""name"":\s*""([^""]*)""");
            MatchCollection matches = regex.Matches(jsonFile);
            
            for (int i = 0; i < 35; i++)
            {
                Random rnd = new Random();
                int musicIndex  = rnd.Next(1, matches.Count);
            
                SetClipboard(matches[musicIndex].Groups[1].Value);
            
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

        private static void MoveMouse(int positionX, int positionY)
        {
            SetCursorPos(positionX, positionY);

            Console.WriteLine($"Cursor movido para a posição ({positionX}, {positionY})");
            
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

