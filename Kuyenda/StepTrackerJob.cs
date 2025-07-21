using Plugin.Maui.Pedometer;
using Shiny.Jobs;
using Kuyenda.Models;
using Kuyenda.Services;

namespace Kuyenda;

public class StepTrackerJob : IJob
{
    private readonly IPedometer pedometer;
    private readonly StepDatabase stepDatabase;

    public StepTrackerJob(IPedometer pedometer)
    {
        this.pedometer = pedometer;
        this.stepDatabase = new StepDatabase(); // Consider using dependency injection if preferred
    }

    public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
        pedometer.ReadingChanged += async (sender, reading) =>
        {
            var currentSteps = reading.NumberOfSteps;
            var todayString = DateTime.Today.ToString("yyyy-MM-dd");

            var existingEntry = await stepDatabase.GetStepByDateAsync(DateTime.Today);

            if (existingEntry != null)
            {
                existingEntry.Steps = currentSteps;
                await stepDatabase.SaveStepAsync(existingEntry);
            }
            else
            {
                var newEntry = new StepModel
                {
                    Date = todayString,
                    Steps = currentSteps
                };
                await stepDatabase.SaveStepAsync(newEntry);
            }
        };

        // Start tracking
        pedometer.Start();

        try
        {
            // Run for 15 minutes
            await Task.Delay(TimeSpan.FromMinutes(15), cancelToken);
        }
        finally
        {
            pedometer.Stop();
        }
    }
}
