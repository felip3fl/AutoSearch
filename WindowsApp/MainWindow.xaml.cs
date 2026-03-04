using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Graphics;
using Windows.UI.WindowManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowsApp
{

    public sealed partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);
        public MainViewModel ViewModel { get; set; } = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();

            TurnOnExtendsContentIntoTitleBar();
            FixWindowSize();
            CenterWindow();
            LoadingSelectedList();
        }

        private void TurnOnExtendsContentIntoTitleBar() {
            ExtendsContentIntoTitleBar = true;
        }

        private void LoadingSelectedList()
        {
            cmbSearchList.SelectedItem = ViewModel.SelectedSearchListOption;
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

            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            

            double scale = GetDpiForWindow(hWnd) / 96.0;


            appWindow.Resize(new Windows.Graphics.SizeInt32
            {
                Width = (int)(700 * scale),
                Height = (int)(475 * scale)
            });

            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsResizable = false;
                presenter.IsMaximizable = false;
            }
        }


        private async Task ShowDialog(string title, string message, string primaryButtonText = "OK", string closeButtonText = "Fechar")
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = primaryButtonText,
                CloseButtonText = closeButtonText,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }

}
