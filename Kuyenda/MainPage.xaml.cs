using Kuyenda.ViewModels;
using Plugin.Maui.Pedometer;
using Shiny;
using Shiny.Jobs;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kuyenda
{
    public partial class MainPage : ContentPage
    {
        private int stepCount = Preferences.Default.Get("CurrentStepCount", 0);
        private int stepGoal = 10000;
        private readonly IPedometer pedometer;
        readonly IJobManager jobManager;

        public MainPage(IPedometer pedometer, IJobManager jobManager)
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
            this.jobManager = jobManager;
            this.pedometer = pedometer;

            // Set initial UI values
            UpdateStepUI(stepCount);            
            // Start pedometer tracking
            pedometer.Start();
        }


        private void UpdateStepUI(int steps)
        {            
            // Calculate and update progress
            double progress = (double)steps / stepGoal;
            
            // Update percentage text
            int percentage = (int)(progress * 100);
    
            
            // Announce for accessibility
            SemanticScreenReader.Announce($"{steps} steps. {percentage} percent of your daily goal.");
        }


        private void OnResetClicked(object? sender, EventArgs e)
        {
            // In a real app, you might want to confirm before resetting
            stepCount = 0;
            UpdateStepUI(0);
        }

        public string JobName { get; set; } = "step-tracker-job";
        public bool BatteryNotLow { get; set; } = true;
        public bool DeviceCharging { get; set; } = false;
        public bool RunOnForeground { get; set; } = true;
        public bool IsInternetRequired { get; set; } = false;


        async Task RegisterJob()
        {
            var access = await jobManager.RequestAccess();
            if (access != AccessState.Available)
            {
                await Shell.Current.DisplayAlert("Error", $"Job permission denied: {access}", "OK");
                return;
            }

            var job = new JobInfo(
                        this.JobName.Trim(),
                        typeof(StepTrackerJob),
                        BatteryNotLow: this.BatteryNotLow,
                        DeviceCharging: this.DeviceCharging,
                        RunOnForeground: true,
                        RequiredInternetAccess: this.IsInternetRequired ? InternetAccess.Any : InternetAccess.None
                    );
            this.jobManager.Register(job);

            await Shell.Current.DisplayAlert("Job Registered", $"Job '{JobName}' has been registered.", "OK");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RegisterJob();
            pedometer.Start();
        }
    }
}
