using System.Runtime.InteropServices;

namespace Search.Tools;

public class MouseTools
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x02; // Clique esquerdo pressionado
    private const uint MOUSEEVENTF_LEFTUP = 0x04;   // Clique esquerdo liberado

    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);


    public void MoveMouse(int positionX, int positionY)
    {
        SetCursorPos(positionX, positionY);
    }

    public void MoveMouse(int positionX, int positionY, int threadSleep)
    {
        MoveMouse(positionX, positionY);
    }

    public void MouseClick(int positionX, int positionY)
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, positionX, positionY, 0, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_LEFTUP, positionX, positionY, 0, UIntPtr.Zero);
    }

    public void MouseClick(int positionX, int positionY, int threadSleep)
    {
        MouseClick(positionX, positionY);
        System.Threading.Thread.Sleep(threadSleep);
    }

}
