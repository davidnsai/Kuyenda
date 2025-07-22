using Plugin.Maui.Pedometer;
using Shiny.Jobs;
using Kuyenda.Services;
using Kuyenda.Models;

namespace Kuyenda.Jobs
{
    public class StepCountingJob : IJob
    {
        private readonly IPedometer _pedometer;
        private readonly StepDatabase _stepDatabase;
        private const string LastStepCountKey = "last_step_count";
        
        public StepCountingJob(IPedometer pedometer, StepDatabase stepDatabase)
        {
            _pedometer = pedometer;
            _stepDatabase = stepDatabase;
        }
        
        public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
        {
            if (!_pedometer.IsSupported)
                return;
                
            try
            {
                // Start pedometer if not already running
                if (!_pedometer.IsMonitoring)
                {
                    _pedometer.Start();
                    await Task.Delay(1000); // Give it time to initialize
                }
                
                // Get current step count
                var currentStepCount = 0;
                var tcs = new TaskCompletionSource<int>();
                
                void OnReadingChanged(object? sender, PedometerData data)
                {
                    currentStepCount = data.NumberOfSteps;
                    tcs.TrySetResult(currentStepCount);
                }
                
                _pedometer.ReadingChanged += OnReadingChanged;
                
                // Wait for reading or timeout
                var readingTask = tcs.Task;
                var timeoutTask = Task.Delay(5000);
                var completedTask = await Task.WhenAny(readingTask, timeoutTask);
                
                _pedometer.ReadingChanged -= OnReadingChanged;
                
                if (completedTask == readingTask)
                {
                    // Get last recorded step count
                    var lastStepCount = Preferences.Get(LastStepCountKey, 0);
                    var lastResetDate = Preferences.Get("last_reset_date", DateTime.MinValue.ToString());
                    
                    // Check if we need to reset (new day)
                    if (DateTime.TryParse(lastResetDate, out var lastReset) && lastReset.Date < DateTime.Today)
                    {
                        lastStepCount = 0;
                        Preferences.Set("last_reset_date", DateTime.Today.ToString());
                    }
                    
                    // Calculate steps taken since last check
                    var stepsSinceLastCheck = currentStepCount - lastStepCount;
                    
                    if (stepsSinceLastCheck > 0)
                    {
                        // Update today's steps in database
                        var todaySteps = await _stepDatabase.GetStepByDateAsync(DateTime.Today);
                        if (todaySteps != null)
                        {
                            todaySteps.Steps += stepsSinceLastCheck;
                            await _stepDatabase.SaveStepAsync(todaySteps);
                        }
                        else
                        {
                            await _stepDatabase.SaveStepAsync(new StepModel
                            {
                                Steps = stepsSinceLastCheck,
                                Date = DateTime.Today.ToString("yyyy-MM-dd")
                            });
                        }
                        
                        // Update last step count
                        Preferences.Set(LastStepCountKey, currentStepCount);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"StepCountingJob error: {ex.Message}");
            }
        }
    }
}
