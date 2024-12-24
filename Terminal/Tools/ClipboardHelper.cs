namespace AutoSearch.Tools;

public class ClipboardHelper
{
    public void SetTextClipboard(string text)
    {
        var clipboardThread = new Thread(() => Clipboard.SetText(text));
        clipboardThread.SetApartmentState(ApartmentState.STA); 
        clipboardThread.Start();
        clipboardThread.Join();
    }
}