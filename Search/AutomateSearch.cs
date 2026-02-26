using Search.Models;
using Search.Tools;
using System.Text.RegularExpressions;
using TextCopy;

namespace Search;

public class AutomateSearch
{
    static List<int> excludedNumbers = new();
    static MouseTools mouseTools = new();
    static ClipboardHelper clipboard = new();
    static KeyTools keyTools = new();
    static bool IsRunning = true;

    public event EventHandler processCompleted;
    public event EventHandler<string> runningSearch;

    public string DrawName(List<string> listOfSearch)
    {
        Random rnd = new Random();

        int listIndex = rnd.Next(1, listOfSearch.Count());

        if (CheckExcludedNumbers(listIndex))
            return DrawName(listOfSearch);

        AddExcludedNumbers(listIndex);

        var selectedValue = listOfSearch[listIndex];

        if (ContainsNonAlphabeticalCharacters(selectedValue))
            return DrawName(listOfSearch);

        return selectedValue;
    }

    private static bool CheckExcludedNumbers(int number)
    {
        if (excludedNumbers.Contains(number))
        {
            return true;
        }
        return false;
    }

    public static bool ContainsNonAlphabeticalCharacters(string input)
    {
        var isMatch = !Regex.IsMatch(input, @"^[a-zA-Z"",.()!?'\-\s]*$");
        return isMatch;
    }

    private static void AddExcludedNumbers(int numberToExclude)
    {
        excludedNumbers.Add(numberToExclude);
    }

    public async void SearchAsync(List<string> listOfSearchText, int timeInterval)
    {
        var threadSleep = 5;
        foreach (var item in listOfSearchText)
        {
            Thread.Sleep(threadSleep * 1000);
            threadSleep = timeInterval;

            if (!IsRunning)
            {
                threadSleep = 0;
                return;
            }

            runningSearch?.Invoke(this, item);
            SearchAndUpdatePage(item, timeInterval);
        }

        processCompleted?.Invoke(this, EventArgs.Empty);

    }

    public void SearchAndUpdatePage(string selectedValue, int inverval)
    {
        var mousePositionX = 260;
        var mousePositiony = 130;

        clipboard.SetTextClipboard(selectedValue);

        mouseTools.MoveMouse(mousePositionX, mousePositiony, 500);
        mouseTools.MouseClick(mousePositionX, mousePositiony, 200);

        keyTools.SendCtrlA();
        keyTools.SendCtrlV();
        keyTools.SendEnter();

    }

}


