namespace AutoSearch.Tools;

public class ClipboardHelper
{
    public void SetTextClipboard(string text)
    {
        var clipboardThread = new Thread(() => ClipboardSetText(text));
        clipboardThread.SetApartmentState(ApartmentState.STA); 
        clipboardThread.Start();
        clipboardThread.Join();
        clipboardThread.Abort();
    }

    private void ClipboardSetText(object text)
    {
        try
        {
            Clipboard.SetText((string)text);
        }
        catch (Exception)
        {
            var tryAgainInSeconds = 4;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error on set clipboard. Tring again in {tryAgainInSeconds} seconds");
            Console.ForegroundColor = default;
            Thread.Sleep(1000 * tryAgainInSeconds);
            ClipboardSetText(text);
        }
    }
}