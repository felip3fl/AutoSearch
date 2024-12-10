using System;
using System.Globalization;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoSearch
{
    public class AutoSearch
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        
        static void Main(string[] args)
        {
            MoveMouse(300,30);
            
            SendKeys.SendWait("^v");
            SendKeys.SendWait("{ENTER}");
        }

        private static void MoveMouse(int positionX, int positionY)
        {
            SetCursorPos(positionX, positionY);

            Console.WriteLine($"Cursor movido para a posição ({positionX}, {positionY})");
        }

    }
}

