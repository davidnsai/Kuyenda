using DevExpress.Maui.Controls;
using Kuyenda.ViewModels;

namespace Kuyenda.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();
        BindingContext = settingsViewModel;
    }

    private void StepGoalTapped(object sender, System.ComponentModel.HandledEventArgs e)
    {
        bottomSheet.State = BottomSheetState.HalfExpanded;
    }

    private void OnSaveStepGoalClicked(object sender, EventArgs e)
    {
        bottomSheet.State = BottomSheetState.Hidden;
    }
}
