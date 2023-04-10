using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Platform;
//using Microsoft.Maui.Graphics.Platform;
using SmartLight.Models;
using SmartLight.ViewModel;

namespace SmartLight;

public partial class DevicePage : ContentPage
{

	
	public DevicePage(DevicePageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
        

    }

	private void ColorPicker_PickedColorChanged(object sender, ColorPicker.Maui.PickedColorChangedEventArgs e)
	{
		Color cpCol = e.NewPickedColorValue;
	}

    private void EnableTimer_Pressed(object sender, EventArgs e)
    {
        TimeSpan timeOff = TimeOff.Time;
		TimeSpan timeOn = TimeOn.Time;

    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}