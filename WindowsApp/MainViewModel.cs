using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFile;
using LocalFile.Models;
using Microsoft.UI.Xaml;
using Newtonsoft.Json;
using Search;
using Search.Models;
using Search.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Input.Preview.Injection;

namespace WindowsApp
{
    public partial class MainViewModel : ObservableObject
    {
        static JsonLocalFile localFile = new();

        [ObservableProperty]
        public partial string Status { get; set; } = "Start";

        [ObservableProperty]
        public partial bool IsRunning { get; set; } = false;

        [ObservableProperty]
        public List<Record> searchListOption = localFile.GetListFileName("Lists\\v1");

        private InputInjector _inputInjector;
        static ClipboardHelper clipboard = new();
        static MouseTools mouseTools = new();
        private CmdExecutor _cmdExecutor = new();


        [RelayCommand]
        public void UpdateStatus()
        {
            Status = "Stop running";
        }

        public void UpdateRunningStatus()
        {
            Status = "Start";

            if (IsRunning)
                Status = "Stop search";
        }

        public MainViewModel()
        {
            InitializeInputInjector();
            UpdateRunningStatus();
        }

        private void InitializeInputInjector()
        {
            _inputInjector = InputInjector.TryCreate();

            if (_inputInjector == null)
                Console.WriteLine("Failed to create InputInjector instance.");

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

        private async void SearchAsync(List<string> listOfSearchText, int timeInterval)
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
                
                SearchAndUpdatePage(item, timeInterval);
            }

        }

        private void SearchAndUpdatePage(string selectedValue, int inverval)
        {
            var mousePositionX = 240;
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

        private bool IsShiftRequired(char c)
        {
            return c switch
            {
                '!' or '@' or '#' or '$' or '%' or '&' or '*' or '(' or ')' => true,
                '_' or '+' or ':' or '?' => true,
                _ => false
            };
        }

        private async void InjectCtrlC_Click(object sender, RoutedEventArgs e)
        {
            InjectKeyCombo(VirtualKey.Control, VirtualKey.C);
        }

        private void InjectCtrlV_Click()
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

        
        public async Task StartSearch(List<string> listOfSearchText, int timeInterval)
        {
            IsRunning = !IsRunning;
            UpdateRunningStatus();

            if (!IsRunning)
            {
                _cmdExecutor.StopShutDownComputer();
                return;
            }

            var timeToFinishSearch = (listOfSearchText.Count * timeInterval) + 5;
            _cmdExecutor.ShutDownComputer(timeToFinishSearch);

            Task.Run(async () => SearchAsync(listOfSearchText, timeInterval));
                
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
    }
}