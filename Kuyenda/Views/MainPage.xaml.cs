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
            
            viewModel = new MainViewModel(stepCountingService);
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();            
            
            await viewModel.InitializeAsync();
            viewModel.StartCountingAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.StopCountingAsync();
        }
    }
}
