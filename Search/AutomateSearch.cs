using Search.Models;
using System.Text.RegularExpressions;

namespace Search
{
    public class AutomateSearch
    {
        static List<int> excludedNumbers = new();
        public string DrawName(ListOfSearch listOfSearch)
        {
            Random rnd = new Random();

            int musicIndex = rnd.Next(1, listOfSearch.Name.Count());

            if (CheckExcludedNumbers(musicIndex))
                return DrawName(listOfSearch);

            AddExcludedNumbers(musicIndex);

            var selectedValue = listOfSearch.Name[musicIndex];

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

    }

}
