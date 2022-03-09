using System;
using Microsoft.Extensions.Logging;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using TestClassLibrary;

namespace MyAppNavS
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {

        public static Window MainWindow { get; internal set; }
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            Page v = (Page)new MyPage();
            Class1 test = new Class1();
            if (Window.Current == null)
            {
                this.m_window = new Window();
                MainWindow = this.m_window;
                MainWindow.Content = (UIElement)v;
                MainWindow.Activate();
            }
            else
            {
                MainWindow = Window.Current;
                MainWindow.Content = (UIElement)v.Content;
                MainWindow.Activate();
            }
        }
        private Window m_window;
    }
}
