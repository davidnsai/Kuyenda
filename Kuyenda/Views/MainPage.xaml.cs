using Kuyenda.ViewModels;
using Kuyenda.Services;

namespace Kuyenda.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel viewModel;

        public MainPage(StepCountingService stepCountingService)
        {
            InitializeComponent();

            if (Application.Current != null)
            {
                // Set theme based on user preference at startup
                var darkMode = Preferences.Get("DarkMode", false);
                Application.Current.UserAppTheme = darkMode ? AppTheme.Dark : AppTheme.Light;
            }

            viewModel = new MainViewModel(stepCountingService);
            
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();            
            
            await viewModel.InitializeAsync();
            viewModel.StartCountingAsync();
        }
    }
}
