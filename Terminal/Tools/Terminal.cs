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
    }
}
