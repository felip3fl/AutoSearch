using LocalFile.Models;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Newtonsoft.Json;
using Search;
using Search.Models;
using Search.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics;
using Windows.System;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.Input.Preview.Injection;
using Windows.UI.WindowManagement;
using static System.Net.WebRequestMethods;
using System.Threading;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsApp
{

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
            LoadingSelectedList();
        }


        private void LoadingSelectedList()
        {
            DateTime todayDate = DateTime.Now;
            var dayOfWeek = (int)todayDate.DayOfWeek;
            cmbSearchList.SelectedItem = ViewModel.searchListOption[dayOfWeek];
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



        private async Task StartSearch()
        {
            var listNumber = cmbSearchList.SelectedItem as Record;
            List<string> listOfSearchText = new();

            if(listNumber == null)
                return;

            var listName = GetRecordById(ViewModel.SearchListOption, listNumber.Id);
            
            var numbersOfSearchesString = Int32.Parse(nbHowManySearch.Text);

            var typeSearch = tgsUpdatePage.GetValue;

            var timeInterval = Int32.Parse(nbTimeBetweenSearch.Text);

            var jsonFile = System.IO.File.ReadAllText(listName.Path);
            var listOfSearch = JsonConvert.DeserializeObject<ListOfSearch>(value: jsonFile);

            for (int i = 0; i < numbersOfSearchesString; i++)
            {
                listOfSearchText.Add(automateSearch.DrawName(listOfSearch));
            }

            await ViewModel.StartSearch(listOfSearchText, timeInterval);
        }



        private static Record GetRecordById(List<Record> files, int Id)
        {
            var listName = files.Where(x => x.Id == Id).FirstOrDefault();
            return listName;
        }
    }

}
