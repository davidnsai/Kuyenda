namespace Kuyenda.ViewModels
{
    public class MainViewModel
    {
        public int Steps { get; set; } = Preferences.Default.Get("CurrentStepCount", 0);
    }
}
