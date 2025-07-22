using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kuyenda.Services;

namespace Kuyenda.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly StepCountingService _stepService;

        public SettingsViewModel(StepCountingService stepService)
        {
            _stepService = stepService;

            // Initialise values from service/state
            StepGoal = _stepService.StepGoal;
            DarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
        }

        [ObservableProperty]
        private bool darkMode;

        [ObservableProperty]
        private int stepGoal;

        [RelayCommand]
        private async Task SaveStepGoal()
        {
            _stepService.StepGoal = StepGoal;
            Preferences.Set("StepGoal", StepGoal);

            CancellationTokenSource cancellationTokenSource = new();

            string text = "Step goal updated!";
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;

            var toast = Toast.Make(text, duration, fontSize);

            await toast.Show(cancellationTokenSource.Token);
        }

        partial void OnDarkModeChanged(bool value)
        {
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
                Preferences.Set("DarkMode", value);
            }
        }

        partial void OnStepGoalChanged(int value)
        {
            _stepService.StepGoal = value;
        }
    }
}
