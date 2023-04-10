using ColorPicker.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLight.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Device = SmartLight.Models.Device;
using DeviceService = SmartLight.Services.DeviceService;
using DeviceDB = SmartLight.DeviceDB;
using CommunityToolkit.Mvvm.Messaging;
using SmartLight.Messages;

#if ANDROID
using SmartLight.Platforms.Android;
using DJ = SmartLight.Platforms.Android.LightOffJob;
#endif


namespace SmartLight.ViewModel;

[QueryProperty("Device", "Device")]
public partial class DevicePageViewModel : BaseViewModel
{

    public DevicePageViewModel()
	{
        WeakReferenceMessenger.Default.Register<UpdateDevice>(this, (r, m) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("Message Rec'd in DPVM");
                device.Power = true;
            });

        });

    }

    [ObservableProperty]
    Device device;

    

    [RelayCommand]
    void BrightnessChanged(ValueChangedEventArgs e)
    {


        if (e is null)
        {
            return;
        }

        

        // Brightness Value (0-100)
        var newValue = e.NewValue;

        // Convert to (0-65535) value
        //int brightnessValue = Convert.ToInt32(newValue * 655.35);
        int brightnessValue = Convert.ToInt32(newValue);

        // Create Packet
        Packet brightnessPacket = new Packet();
        brightnessPacket.SetColourPacket(device.TargetBin, device.Hue.ToString(), device.Saturation.ToString(), brightnessValue.ToString(), device.Kelvin.ToString(), "500");

        // Send packet to device
        DeviceService.BroadcastPackets(brightnessPacket.FullPacket, device.IpAddress);
        
    }


    [RelayCommand]
    void ColorChanged(PickedColorChangedEventArgs e)
    {

        if (e is null)
        {
            return;
        }



        // Temporary fix which fixes a bug which occurs when the DevicePage.xaml Color Picker.PickedColor property converter is fired
        // and returns a null value.

        if (e.NewPickedColorValue.ToHex() == "#000000")
        {
            return;
        }
     
        
        var cpChange = e.NewPickedColorValue;

        Debug.WriteLine(cpChange);

        Debug.WriteLine("HEX");
        Debug.WriteLine(cpChange.ToHex());

        var hsb = Helper.RgbToHsb(cpChange.ToHex());

        Debug.WriteLine("Hue:");
        Debug.WriteLine(hsb[0]);

        Debug.WriteLine("Saturation:");
        Debug.WriteLine(hsb[1]);

        Debug.WriteLine("Brightness:");
        Debug.WriteLine(hsb[2]);

        // Convert Vals to Lifx Representation
        int UIntHue = Helper.UIntHue(hsb[0]);
        Debug.WriteLine("LIFX HUE");
        Debug.WriteLine(UIntHue.ToString());
        device.Hue = UIntHue;

        int UIntSat = Helper.UIntSB(hsb[1]);
        Debug.WriteLine("LIFX SAT");
        Debug.WriteLine(UIntSat.ToString());
        device.Saturation = UIntSat;

        int UIntBrightness = Helper.UIntSB(hsb[2]);
        Debug.WriteLine("LIFX BRIGHT");
        Debug.WriteLine(UIntBrightness.ToString());
        device.Brightness = UIntBrightness;


        // Kelvins == Warmth

        // Create SetColor Packet
        Packet ColourPacket = new();
        ColourPacket.SetColourPacket(device.TargetBin, UIntHue.ToString(), UIntSat.ToString(), UIntBrightness.ToString(), device.Kelvin.ToString(), "2000");

        // Send Packet to Device
        DeviceService.BroadcastPackets(ColourPacket.FullPacket, device.IpAddress);


    }

    [RelayCommand]
    void KelvinChanged(ValueChangedEventArgs e)
    {
        if (e is null)
        {
            return;
        }

        var kelvin = (int)e.NewValue;

        // Create Packet
        Packet kelvinPacket = new();
        kelvinPacket.SetColourPacket(device.TargetBin, device.Hue.ToString(), device.Saturation.ToString() ,device.Brightness.ToString(), kelvin.ToString(), "2000");

        // Send Packet to Device
        DeviceService.BroadcastPackets(kelvinPacket.FullPacket, device.IpAddress);

    }


    [RelayCommand]
    void TimerEnabled()
    {

        bool ipExists = false;

        Debug.WriteLine("Light will turn on at:");
        Debug.WriteLine(device.TimeOn);

        Debug.WriteLine("Light will turn off at:");
        Debug.WriteLine(device.TimeOff);

        // STORE VALUES IN DATABASE
        // Check if Device IP Address Already Exists --- DELETE LATER DEBUG ONLY
        List<DeviceStorage> deviceStorages = App.DeviceDataBase.GetAllDevices();


        foreach (DeviceStorage dbDevice in deviceStorages)
        {
            Debug.WriteLine(dbDevice.IpAddress);
            // Check if IP Address Match
            if (dbDevice.IpAddress == device.IpAddress)
            {
                ipExists = true;
            }
        }

        if (ipExists)
        {
            // Update Timer Values
            Debug.WriteLine("IP Exists");
            App.DeviceDataBase.UpdateDevice(device.IpAddress, device.TimeOn, device.TimeOff);
            Debug.WriteLine("GS");
        } else
        {
            // Else Add New Entry into Table
            App.DeviceDataBase.AddNewDevice(device.IpAddress, device.TimeOn, device.TimeOff);
            Debug.WriteLine("Entry Added");
        }

#if ANDROID
        TimerAlarm timerAlarm = new();
        timerAlarm.SetTimer(device);
#endif



    }



}

