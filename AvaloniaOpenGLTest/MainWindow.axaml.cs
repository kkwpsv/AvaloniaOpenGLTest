using Avalonia;
using Avalonia.Controls;

namespace AvaloniaOpenGLTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.AttachDevTools();
        }

        private void Window_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            OpenGLTestControl.Focus();
        }
    }
}