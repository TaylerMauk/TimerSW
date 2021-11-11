using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using TimerSW.ViewModels;

namespace TimerSW.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private const int GWL_STYLE = -16;
        private readonly IntPtr WS_MAXIMIZEBOX = (IntPtr)0x10000;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            // Prevent window from being accidentally maximized

            IntPtr hWnd = new WindowInteropHelper((Window)sender).Handle;
            IntPtr windowStyle = GetWindowLongPtr(hWnd, GWL_STYLE);

            IntPtr newWindowStyle;
            if (Environment.Is64BitOperatingSystem)
                newWindowStyle = (IntPtr)((long)windowStyle & ~(long)WS_MAXIMIZEBOX);
            else
                newWindowStyle = (IntPtr)((int)windowStyle & ~(int)WS_MAXIMIZEBOX);

            SetWindowLongPtr(hWnd, GWL_STYLE, newWindowStyle);
        }
    }
}
