using CommunityToolkit.Mvvm.ComponentModel;
using Kuyenda.Models;
using Kuyenda.Services;
using Microsoft.Maui.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kuyenda.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly StepCountingService _stepCountingService;
        
        [ObservableProperty]
        private int steps;
        
        [ObservableProperty]
        private int stepGoal = Preferences.Get("StepGoal", 10000);
        
        [ObservableProperty]
        private double progressPercentage;

        [ObservableProperty]
        private List<StepModel> stepHistory = new();

        public MainViewModel(StepCountingService stepCountingService)
        {
            _stepCountingService = stepCountingService;
            _stepCountingService.PropertyChanged += OnStepCountingServicePropertyChanged;
        }
        
        private void OnStepCountingServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StepCountingService.TodaySteps))
            {
                Steps = _stepCountingService.TodaySteps;
            }
            else if (e.PropertyName == nameof(StepCountingService.ProgressPercentage))
            {
                ProgressPercentage = _stepCountingService.ProgressPercentage;
            }
        }
        
        public async Task InitializeAsync()
        {
            await _stepCountingService.InitializeAsync();
            Steps = _stepCountingService.TodaySteps;
            StepGoal = _stepCountingService.StepGoal;
            ProgressPercentage = _stepCountingService.ProgressPercentage;
            StepHistory = await _stepCountingService.GetLastFiveDaysStepsAsync();
        }
        
        public void StartCountingAsync()
        {
             _stepCountingService.StartCounting();
        }
        
        public void StopCountingAsync()
        {
             _stepCountingService.StopCounting();
        }
    }
}
