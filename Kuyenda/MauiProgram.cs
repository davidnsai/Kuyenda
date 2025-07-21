using DevExpress.Maui;
using DevExpress.Maui.Core;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Pedometer;
using Shiny;

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
                .UseShiny()
                .UseMauiApp<App>()
                .UseDevExpress(useLocalization: false)
                .UseDevExpressCharts()
                .UseDevExpressCollectionView()
                .UseDevExpressControls()
                .UseDevExpressGauges()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("roboto-bold.ttf", "Roboto-Bold");
                    fonts.AddFont("roboto-medium.ttf", "Roboto-Medium");
                    fonts.AddFont("roboto-regular.ttf", "Roboto");
                });

            builder.Services.AddSingleton(Pedometer.Default);
            builder.Services.AddJobs();
            builder.Services.AddSingleton<StepTrackerJob>();

            return builder.Build();
        }
    }
}
