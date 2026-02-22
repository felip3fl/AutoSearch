using TextCopy;

namespace Search.Tools;

public class ClipboardHelper
{
    public void SetTextClipboard(string text)
    {
        ClipboardService.SetText(text);
    }

    public string GetTextClipboard()
    {
        return ClipboardService.GetText();
    }

    private void ClipboardSetText(object text)
    {
        ClipboardService.SetText((string)text);
    }
}