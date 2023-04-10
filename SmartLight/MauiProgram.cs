using SmartLight.Services;
using SmartLight.ViewModel;
using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Helper = SmartLight.Models.Helper;
using Microsoft.Maui.LifecycleEvents;
using static Microsoft.Maui.ApplicationModel.Platform;
using System.ServiceModel.Channels;

#if ANDROID

using Android.App;
using Android.Content;
#endif

namespace SmartLight;
public static class MauiProgram
{

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>().UseSkiaSharp().UseMauiCommunityToolkit().ConfigureFonts(fonts =>
        {
            fonts.AddFont("DMSerifDisplay-Regular.ttf", "DmSerifRegular");
            fonts.AddFont("DDMSerifDisplay-Italic.ttf", "DmSerifItalic");
            fonts.AddFont("Montserrat-VariableFont_wght.ttf", "Montserrat");
            fonts.AddFont("Montserrat-Bold.ttf", "MontserratBold");
            fonts.AddFont("Montserrat-SemiBold.ttf", "MontserratSemiBold");
            fonts.AddFont("Montserrat-Medium.ttf", "MontserratMedium");
            fonts.AddFont("Montserrat-Black.ttf", "MontserratBlack");
            fonts.AddFont("fa-brands-400.ttf", "FontAwesomeBrands");
            fonts.AddFont("fa-regular-400.ttf", "FontAwesomeRegular");
            fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
            fonts.AddFont("Lato-Black.ttf", "LatoBlack");
            fonts.AddFont("Lato-BlackItalic.ttf", "LatoBlackItalic");
            fonts.AddFont("Lato-Bold.ttf", "LatoBold");
            fonts.AddFont("Lato-BoldItalic.ttf", "LatoBoldItalic");
            fonts.AddFont("Lato-Italic.ttf", "LatoItalic");
            fonts.AddFont("Lato-Light.ttf", "LatoLight");
            fonts.AddFont("Lato-LightItalic.ttf", "LatoLightItalic");
            fonts.AddFont("Lato-Regular.ttf", "LatoRegular");
            fonts.AddFont("Lato-Thin.ttf", "LatoThin");
            fonts.AddFont("Lato-ThinItalic.ttf", "LatoThinItalic");
        });




        builder.Services.AddSingleton<DeviceService>();



        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<DevicePage>();

        builder.Services.AddSingleton<DeviceViewModel>();
        builder.Services.AddTransient<DevicePageViewModel>();

        // DB Code
        string dbPath = Helper.GetLocalFilePath("devices.db3");
        builder.Services.AddSingleton<DeviceDB>(s => ActivatorUtilities.CreateInstance<DeviceDB>(s, dbPath));


        return builder.Build();
    }
}