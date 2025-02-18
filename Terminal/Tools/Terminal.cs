using System.Reflection;
using System.Runtime.InteropServices;

namespace AutoSearch.Tools
{
    public class Terminal
    {
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
        public const int SW_RESTORE = 9;
        public void SetFocus()
        {
            string originalTitle = Console.Title;
            string uniqueTitle = Guid.NewGuid().ToString();
            Console.Title = uniqueTitle;
            Thread.Sleep(50);
            IntPtr handle = FindWindowByCaption(IntPtr.Zero, uniqueTitle);

            Console.Title = originalTitle;

            ShowWindowAsync(new HandleRef(null, handle), SW_RESTORE);
            SetForegroundWindow(handle);
        }

        public void PrintFL()
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

        public string GetProjectVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version.ToString();
        }

        public void DefineConsoletitle(string title)
        {
            Console.Title = title;
        }
    }
}
