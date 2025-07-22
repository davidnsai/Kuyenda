using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Kuyenda.Models
{
    public class StepInfo : INotifyPropertyChanged
    {
        private int _steps;
        public int Steps
        {
            get { return _steps; }
            set
            {
                _steps = value;
                OnPropertyChanged();
            }
        }
        private double _percentage;
        public double Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler ?PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
