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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsApp
{

    public sealed partial class MainWindow : Window
    {
        static AutomateSearch automateSearch = new();
        static ClipboardHelper clipboard = new();
        static MouseTools mouseTools = new();
        
        private InputInjector _inputInjector;

        public MainViewModel ViewModel { get; set; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;

            CenterWindow();
            FixWindowSize();
            InitializeInputInjector();
            LoadingSelectedList();
        }


        private void LoadingSelectedList()
        {
            DateTime todayDate = DateTime.Now;
            var dayOfWeek = (int)todayDate.DayOfWeek;
            cmbSearchList.SelectedItem = ViewModel.searchListOption[dayOfWeek];
        }

        private void InitializeInputInjector()
        {
            try
            {
                
                _inputInjector = InputInjector.TryCreate();

                if (_inputInjector == null)
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
        }

        private VirtualKey CharToVirtualKey(char c)
        {
            char upperChar = char.ToUpper(c);

            // Letras A-Z
            if (upperChar >= 'A' && upperChar <= 'Z')
            {
                return (VirtualKey)(upperChar);
            }

            // Números 0-9
            if (c >= '0' && c <= '9')
            {
                return (VirtualKey)(VirtualKey.Number0 + (c - '0'));
            }

            // Espaço
            if (c == ' ')
            {
                return VirtualKey.Space;
            }

            // Caracteres especiais comuns
            return c switch
            {
                '!' => VirtualKey.Number1,
                '@' => VirtualKey.Number2,
                '#' => VirtualKey.Number3,
                '$' => VirtualKey.Number4,
                '%' => VirtualKey.Number5,
                '&' => VirtualKey.Number7,
                '*' => VirtualKey.Number8,
                '(' => VirtualKey.Number9,
                ')' => VirtualKey.Number0,
                '-' => VirtualKey.Subtract,
                '_' => VirtualKey.Subtract,
                '=' => (VirtualKey)187, // Equal/Plus key
                '+' => (VirtualKey)187,
                '.' => (VirtualKey)190,
                ',' => (VirtualKey)188,
                ';' => (VirtualKey)186,
                ':' => (VirtualKey)186,
                '/' => (VirtualKey)191,
                '?' => (VirtualKey)191,
                _ => VirtualKey.Space // Fallback
            };
        }

        private bool IsShiftRequired(char c)
        {
            return c switch
            {
                '!' or '@' or '#' or '$' or '%' or '&' or '*' or '(' or ')' => true,
                '_' or '+' or ':' or '?' => true,
                _ => false
            };
        }

        private void InjectText(string text)
        {
            var keyboardInfo = new List<InjectedInputKeyboardInfo>();

            foreach (char c in text)
            {
                // Converte o caractere para VirtualKey
                VirtualKey virtualKey = CharToVirtualKey(c);
                bool needsShift = char.IsUpper(c) || IsShiftRequired(c);

                if (needsShift)
                {
                    // Pressiona Shift
                    keyboardInfo.Add(new InjectedInputKeyboardInfo
                    {
                        VirtualKey = (ushort)VirtualKey.Shift,
                        KeyOptions = InjectedInputKeyOptions.None
                    });
                }

                // Pressiona a tecla
                keyboardInfo.Add(new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)virtualKey,
                    KeyOptions = InjectedInputKeyOptions.None
                });

                // Solta a tecla
                keyboardInfo.Add(new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)virtualKey,
                    KeyOptions = InjectedInputKeyOptions.KeyUp
                });

                if (needsShift)
                {
                    // Solta Shift
                    keyboardInfo.Add(new InjectedInputKeyboardInfo
                    {
                        VirtualKey = (ushort)VirtualKey.Shift,
                        KeyOptions = InjectedInputKeyOptions.KeyUp
                    });
                }
            }

            _inputInjector.InjectKeyboardInput(keyboardInfo);
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

        private void InjectKeyCombo(VirtualKey modifierKey, VirtualKey key)
        {
            var keyboardInfo = new List<InjectedInputKeyboardInfo>
            {
                // Pressiona modificador (Ctrl, Alt, etc)
                new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)modifierKey,
                    KeyOptions = InjectedInputKeyOptions.None
                },
                // Pressiona a tecla principal
                new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)key,
                    KeyOptions = InjectedInputKeyOptions.None
                },
                // Solta a tecla principal
                new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)key,
                    KeyOptions = InjectedInputKeyOptions.KeyUp
                },
                // Solta o modificador
                new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)modifierKey,
                    KeyOptions = InjectedInputKeyOptions.KeyUp
                }
            };

            _inputInjector.InjectKeyboardInput(keyboardInfo);
        }

        private async void InjectCtrlC_Click(object sender, RoutedEventArgs e)
        {
            InjectKeyCombo(VirtualKey.Control, VirtualKey.C);
        }

        private  void InjectCtrlV_Click()
        {
            InjectKeyCombo(VirtualKey.Control, VirtualKey.V);
        }

        private void InjectCtrlA_Click()
        {
            InjectKeyCombo(VirtualKey.Control, VirtualKey.A);
        }

        private void InjectEnter_Click()
        {
            InjectKey(VirtualKey.Enter);
        }

        private void InjectKey(VirtualKey key)
        {
            var keyboardInfo = new List<InjectedInputKeyboardInfo>
            {
                // Pressiona a tecla
                new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)key,
                    KeyOptions = InjectedInputKeyOptions.None
                },
                // Solta a tecla
                new InjectedInputKeyboardInfo
                {
                    VirtualKey = (ushort)key,
                    KeyOptions = InjectedInputKeyOptions.KeyUp
                }
            };

            _inputInjector.InjectKeyboardInput(keyboardInfo);
        }

        private void Search()
        {

            var listNumber = cmbSearchList.SelectedItem as Record;

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
                var selectedValue = automateSearch.DrawName(listOfSearch);

                PrintDateTime();

                SearchAndUpdatePage(selectedValue, timeInterval);

                System.Threading.Thread.Sleep(timeInterval * 100);
            }

            //Console.Write($"\nFINISH - Total time: {watch.getTotalTime()}");

            //OpenPointPage();
        }

        private static void PrintDateTime()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{DateTime.Now.ToLongTimeString()}] ");
            Console.ResetColor();
        }

        private void SearchAndUpdatePage(string selectedValue, int inverval)
        {
            var mousePositionX = 225;
            var mousePositiony = 130;

            clipboard.SetTextClipboard(selectedValue);

            mouseTools.MoveMouse(mousePositionX, mousePositiony, 500);
            mouseTools.MouseClick(mousePositionX, mousePositiony, 2000);

            InjectCtrlA_Click();
            System.Threading.Thread.Sleep(200);

            InjectCtrlV_Click();
            System.Threading.Thread.Sleep(200);

            InjectEnter_Click();
            System.Threading.Thread.Sleep(200);

        }

        private static Record GetRecordById(List<Record> files, int Id)
        {
            var listName = files.Where(x => x.Id == Id).FirstOrDefault();
            return listName;
        }
    }

}
