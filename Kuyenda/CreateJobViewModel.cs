using Shiny;
using Shiny.Jobs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Kuyenda;

public class CreateJobViewModel : INotifyPropertyChanged
{
    readonly IJobManager jobManager;
    public ICommand RegisterJobCommand { get; }

    public CreateJobViewModel(IJobManager jobManager)
    {
        this.jobManager = jobManager;

        RegisterJobCommand = new Command(async () => await RegisterJob());
    }

    // Sample configurable properties
    public string JobName { get; set; } = "step-tracker-job";
    public bool BatteryNotLow { get; set; } = true;
    public bool DeviceCharging { get; set; } = false;
    public bool RunOnForeground { get; set; } = true;
    public bool IsInternetRequired { get; set; } = false;

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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
                    RunOnForeground: this.RunOnForeground,
                    RequiredInternetAccess: this.IsInternetRequired ? InternetAccess.Any : InternetAccess.None
                );
        this.jobManager.Register(job);

        await Shell.Current.DisplayAlert("Job Registered", $"Job '{JobName}' has been registered.", "OK");
    }
}
