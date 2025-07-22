using CommunityToolkit.Maui;
using DevExpress.Maui;
using DevExpress.Maui.Core;
using Kuyenda.Services;
using Kuyenda.ViewModels;
using Plugin.Maui.Pedometer;

namespace Kuyenda
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            ThemeManager.ApplyThemeToSystemBars = true;
            ThemeManager.UseAndroidSystemColor = false;
            ThemeManager.Theme = new Theme(Color.FromArgb("FFAFFA01"));

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseDevExpress(useLocalization: false)
                .UseDevExpressCharts()
                .UseDevExpressCollectionView()
                .UseDevExpressControls()
                .UseDevExpressEditors()
                .UseDevExpressGauges()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("roboto-bold.ttf", "Roboto-Bold");
                    fonts.AddFont("roboto-medium.ttf", "Roboto-Medium");
                    fonts.AddFont("roboto-regular.ttf", "Roboto");
                });

            builder.Services.AddSingleton(Pedometer.Default);
            builder.Services.AddSingleton<StepDatabase>();
            builder.Services.AddSingleton<StepCountingService>();
            builder.Services.AddSingleton<SettingsViewModel>();

            return builder.Build();
        }
    }
}
