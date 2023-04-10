using CommunityToolkit.Maui.Converters;
using SmartLight.ViewModel;
using Microsoft.Maui.Controls.Xaml;
//using Xamarin.Google.Crypto.Tink.Subtle;

namespace SmartLight;

public partial class MainPage : ContentPage
{

	public MainPage(DeviceViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

        

	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        System.Diagnostics.Debug.WriteLine("On the Device Page");
        
    }
}

