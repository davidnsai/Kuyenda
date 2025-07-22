using CommunityToolkit.Mvvm.ComponentModel;
using Kuyenda.Models;
using Plugin.Maui.Pedometer;
using Microsoft.Maui.Storage;

namespace Kuyenda.Services
{
    public partial class StepCountingService : ObservableObject
    {
        private readonly IPedometer _pedometer;
        private readonly StepDatabase _stepDatabase;

        private const string LastStepDateKey = "LastStepDate";
        private const string LastRecordedStepsKey = "LastRecordedSteps";

        public StepCountingService(IPedometer pedometer, StepDatabase stepDatabase)
        {
            _pedometer = pedometer;
            _stepDatabase = stepDatabase;
            _pedometer.ReadingChanged += OnPedometerReadingChanged;
        }

        [ObservableProperty]
        private int todaySteps;

        [ObservableProperty]
        private int stepGoal = Preferences.Get("StepGoal", 10000);

        [ObservableProperty]
        private bool isCountingSteps;

        public double ProgressPercentage => (double)TodaySteps / StepGoal * 100;

        partial void OnTodayStepsChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(ProgressPercentage));
        }

        partial void OnStepGoalChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(ProgressPercentage));
        }

        private DateTime LastStepDate
        {
            get
            {
                var stored = Preferences.Get(LastStepDateKey, string.Empty);
                return string.IsNullOrWhiteSpace(stored) ? DateTime.Today : DateTime.Parse(stored);
            }
            set
            {
                Preferences.Set(LastStepDateKey, value.ToString("yyyy-MM-dd"));
            }
        }

        private int LastRecordedSteps
        {
            get => Preferences.Get(LastRecordedStepsKey, 0);
            set => Preferences.Set(LastRecordedStepsKey, value);
        }

        public async Task InitializeAsync()
        {
            TodaySteps = await GetStepsForDateAsync(DateTime.Today);
            LastStepDate = DateTime.Today; // Reset the saved date on startup
        }

        public void StartCounting()
        {
            if (!_pedometer.IsSupported || IsCountingSteps)
                return;

            _pedometer.Start();
            IsCountingSteps = true;
        }

        public void StopCounting()
        {
            if (!IsCountingSteps)
                return;

            _pedometer.Stop();
            IsCountingSteps = false;
        }

        public async Task<int> GetStepsForDateAsync(DateTime date)
        {
            var stepModel = await _stepDatabase.GetStepByDateAsync(date);
            return stepModel?.Steps ?? 0;
        }

        public async Task<List<StepModel>> GetLastFiveDaysStepsAsync()
        {
            var steps = await _stepDatabase.GetLastFiveDaysStepsAsync();
            return steps;
        }

        public async Task SaveStepsAsync(int steps, DateTime date)
        {
            var existingEntry = await _stepDatabase.GetStepByDateAsync(date);
            if (existingEntry != null)
            {
                existingEntry.Steps = steps;
                await _stepDatabase.SaveStepAsync(existingEntry);
            }
            else
            {
                await _stepDatabase.SaveStepAsync(new StepModel
                {
                    Steps = steps,
                    Date = date.ToString("yyyy-MM-dd")
                });
            }

            if (date.Date == DateTime.Today)
            {
                TodaySteps = steps;
            }
        }

        private async void OnPedometerReadingChanged(object? sender, PedometerData reading)
        {
            try
            {
                var currentDate = DateTime.Today;

                // Midnight check
                if (LastStepDate != currentDate)
                {
                    LastStepDate = currentDate;
                    LastRecordedSteps = reading.NumberOfSteps;

                    // Reset TodaySteps from DB for new day
                    TodaySteps = await GetStepsForDateAsync(currentDate);
                    return;
                }

                var stepDifference = reading.NumberOfSteps - LastRecordedSteps;

                if (stepDifference > 0)
                {
                    LastRecordedSteps = reading.NumberOfSteps;
                    var newTotal = TodaySteps + stepDifference;
                    await SaveStepsAsync(newTotal, currentDate);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Step counting error: {ex.Message}");
            }
        }
    }
}
