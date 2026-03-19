using Microsoft.UI;
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

        private WindowId windowId;
        private Microsoft.UI.Windowing.AppWindow appWindow = null!;
        private IntPtr windowHandle;
        public MainViewModel ViewModel { get; }

        public MainWindow(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            LoadingWindowID();

            TurnOnExtendsContentIntoTitleBar();
            FixWindowSize();
            CenterWindow();
            DisableMaximizeButton();
            WindowAlwaysOnTop();

            LoadingSelectedList();
        }

        private void btnTottleTheme_click(object sender, RoutedEventArgs e)
        {
            root.RequestedTheme = root.RequestedTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
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

        private void WindowAlwaysOnTop()
        {
            if (appWindow.Presenter is OverlappedPresenter overlappedPresenter)
            {
                overlappedPresenter.IsAlwaysOnTop = true;
            }
        }

        private void LoadingWindowID()
        {
            windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        }

        private void DisableMaximizeButton()
        {
            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsResizable = false;
                presenter.IsMaximizable = false;
            }
        }

        private void FixWindowSize()
        {
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            double scale = GetDpiForWindow(windowHandle) / 96.0;
            appWindow.Resize(new Windows.Graphics.SizeInt32
            {
                Width = (int)(700 * scale),
                Height = (int)(475 * scale)
            });

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
