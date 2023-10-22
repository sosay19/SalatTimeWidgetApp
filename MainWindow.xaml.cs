using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using SalatTimeWidgetApp.ViewModels;

namespace SalatTimeWidgetApp
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel;
        private bool isDragging = false;
        private Point offset;
        private DispatcherTimer currentTimeTimer; // Declare a single timer instance
        private DispatcherTimer salatTimesTimer;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            DataContext = viewModel;

            // Initialize a timer to update the current time
            currentTimeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            currentTimeTimer.Tick += (sender, args) =>
            {
                viewModel.CurrentTime = DateTime.Now.ToString("HH:mm:ss - dd ddd MMM");
            };
            currentTimeTimer.Start();

            // Initialize a separate timer to periodically fetch Salat times
            salatTimesTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            salatTimesTimer.Tick += async (sender, args) =>
            {
                await viewModel.UpdateSalatTimesAsync("Brussels", "Belgium", 9);
            };
            salatTimesTimer.Start();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.UpdateSalatTimesAsync("Brussels", "Belgium", 9);
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = true;
                offset = e.GetPosition(this);
                CaptureMouse();
                Cursor = Cursors.Hand;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPoint = e.GetPosition(this);
                Left = currentPoint.X - offset.X + Left;
                Top = currentPoint.Y - offset.Y + Top;
            }
        }
    }
}