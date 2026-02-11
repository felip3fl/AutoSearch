using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Search;
using System;
using System.Collections.Generic;
using Windows.Graphics;
using Windows.UI.WindowManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 


    public sealed partial class MainWindow : Window
    {
        static AutomateSearch automateSearch = new();
        public MainViewModel ViewModel { get; set; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;

            CenterWindow();
            FixWindowSize();
        }



        private void BtnExecutar_Click(object sender, RoutedEventArgs e)
        {
            
            

            //for (int i = 0; i < numbersOfSearchesInt; i++)
            //{
            //    var selectedValue = automateSearch.DrawName(listOfSearch);

            //    PrintDateTime();
            //    Console.WriteLine($"{listName.Name} {i + 1}/{numbersOfSearchesString}: " +
            //                            $"{selectedValue}");

            //    if (typeSearch == "1")
            //        Search(selectedValue, timeInterval);
            //    if (typeSearch == "2")
            //        SearchAndUpdatePage(selectedValue, timeInterval);
            //}
        }


        private void ButtonMoveNavigation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CenterWindow()
        {
            var area = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest)?.WorkArea;
            if (area == null) return;
            AppWindow.Move(new PointInt32((area.Value.Width - AppWindow.Size.Width) / 2, (area.Value.Height - AppWindow.Size.Height) / 2));
        }

        private void FixWindowSize()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 700, Height = 455 });

            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsResizable = false;
                presenter.IsMaximizable = false;
            }
        }

        private void updateRichTextBlock()
        {
            List<string> logItems = new List<string>
            {
                "Pesquisa iniciada: São Paulo",
                "Aguardando 60 segundos...",
                "Pesquisa iniciada: Rio de Janeiro",

            };

            richTextBlock.Blocks.Clear();

            foreach (var item in logItems)
            {
                Paragraph paragraph = new Paragraph();
                Run run = new Run { Text = item };
                paragraph.Inlines.Add(run);
                richTextBlock.Blocks.Add(paragraph);
            }
        }

        private void Search()
        {

            //var listNumber = Int32.Parse(Console.ReadLine());
            //var listName = GetRecordById(files, listNumber);

            //Console.WriteLine("\nHow many searches do you want to do?");
            //var numbersOfSearchesString = Console.ReadLine();

            //Console.WriteLine("\nWhat type of search do you want?");
            //Console.WriteLine("1 Normal (default)");
            //Console.WriteLine("2 Search and update page");
            //var typeSearch = Console.ReadLine();

            //Console.WriteLine("\nTime interval (seconds)? ");
            //var timeInterval = Int32.Parse(Console.ReadLine());

            //int.TryParse(numbersOfSearchesString, out int numbersOfSearchesInt);

            //var jsonFile = File.ReadAllText(listName.Path);
            //var listOfSearch = JsonConvert.DeserializeObject<ListOfSearch>(value: jsonFile);

            //Console.Clear();
            //terminal.PrintFL();

            //watch.Start();

            //for (int i = 0; i < numbersOfSearchesInt; i++)
            //{
            //    var selectedValue = automateSearch.DrawName(listOfSearch);

            //    PrintDateTime();
            //    Console.WriteLine($"{listName.Name} {i + 1}/{numbersOfSearchesString}: " +
            //                            $"{selectedValue}");

            //    if(typeSearch == "1")
            //        Search(selectedValue, timeInterval);
            //    if (typeSearch == "2")
            //        SearchAndUpdatePage(selectedValue, timeInterval);
            //}

            //watch.Stop();
            //Console.Write($"\nFINISH - Total time: {watch.getTotalTime()}");

            //OpenPointPage();
            //Console.ReadLine();
        }
    }

}
