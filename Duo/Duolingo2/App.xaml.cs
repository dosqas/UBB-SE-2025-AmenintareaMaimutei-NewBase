using Microsoft.UI.Xaml;

namespace Duolingo2
{
    public partial class App : Application
    {
        private Window? m_window;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var viewModel = new MainWindowModelView();

            var mainWindow = new MainWindow();
            mainWindow.SetViewModel(viewModel); // creezi o metodă custom

            mainWindow.Activate();
            m_window = mainWindow;
        }
    }
}
