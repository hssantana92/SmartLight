using SmartLight.Services;
using SmartLight.Models;
using System.Collections.ObjectModel;
using Device = SmartLight.Models.Device;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SmartLight.Messages;


namespace SmartLight.ViewModel
{
    public partial class DeviceViewModel : BaseViewModel
    {
        readonly DeviceService deviceService;

        public DeviceViewModel(DeviceService deviceService)
        {
            Title = "Devices";
            this.deviceService = deviceService;
            Devices = new ObservableCollection<Device>();

            HasDevices = false;
            PowerStatus = false;

        }

        [ObservableProperty]
        ObservableCollection<Device> devices;

        [ObservableProperty]
        public bool powerStatus;

        [ObservableProperty]
        bool hasDevices;

        
        // Navigate Method: Allows the device to be passed to the DevicePage View.
        // Input: Selected Light/Device
        // Outputs: Null or a Task object
        [RelayCommand]
        async Task Navigate(Device device)
        {
            if (device is null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(DevicePage)}", true,
                
                new Dictionary<string, object>
                {
                    {"Device", device}
                }
                );
        }

        // Method: Allows the Device View to retrieve/discover all Devices

        [RelayCommand]
        async void GetDevices()
        {
            Remove();

            IsBusy = true;

            var discoverDevices = DeviceService.DiscoverDevices();
            var devicesAwait = await discoverDevices;
            Devices = devicesAwait;

            IsBusy = false;

            if (Devices.Count > 0)
            {
                HasDevices = true;
               
            }
            
        }

        // Clears All Devices from the Device Observable Collection
        [RelayCommand]
        public void Remove()
        {
            Devices.Clear();
        }


        // Sets a label for the selected light/device.
        [RelayCommand]
        static async void SetLabel(Device device)
        {

            // Prompts the user to enter a name for the light
            string result = await App.Current.MainPage.DisplayPromptAsync("Set Label", "Enter a new label for this light");

            if (result != null)
            {
                device.Label = result;
                //Create packet to set label
                Packet setLabel = new();
                setLabel.SetLabelPacket(device.TargetBin, result);

                // Send new label to Device
                _ = DeviceService.SendPacket(setLabel.FullPacket, device.IpAddress);
            } 


        }


        // Turns the light on/off
        [RelayCommand]
        static void TurnOff(Device device)
        {             
            if (device is not null)
            {
                DeviceService.TogglePower(device);
            }
            
        }

        // Sets the brightness for the specified light
        [RelayCommand]
        static void SetBrightness(Device device)
        {
            if (device is not null)
            {

                // Create packet to set label
                Packet setBrightness = new();
                setBrightness.SetColourPacket(device.TargetBin, device.Hue.ToString(), device.Saturation.ToString(), device.Brightness.ToString(), device.Kelvin.ToString(), "1000");

                // Send new label to Device
                _ = DeviceService.SendPacket(setBrightness.FullPacket, device.IpAddress);
            }
        }

        // Turns all lights On or Off depending on each device light status.
        // IF one light is On, all devices will be turned off.
        // IF all lights are off, all devices will be turned on.
        [RelayCommand]
        void PowerAll()
        {
            
            // iterate through each device
            // if any of the power values are true, set the Device View power value to true,
            foreach (var device in Devices)
            {
                if (device.Power)
                {
                    PowerStatus = true;
                }
            }

            if (PowerStatus) // Turn all lights off
            {
                for (int i= 0; i < Devices.Count; i++)
                {
                    Devices[i].Power = false;
                }

                // Reset PowerStatus
                PowerStatus = false;

            } else // Turn all lights on.
            {
                for (int i=0; i < Devices.Count; i++)
                {
                    Devices[i].Power = true;
                }
            }


        }

        
        // Updates Device View once Auto Timer is enabled and has triggered
        [RelayCommand]
        public void UpdateDevice(string ipAddress)
        {

            // Check DB to see if IpAddress exists for Auto Timer
            DeviceStorage dbDevice = App.DeviceDataBase.GetDevice(ipAddress);

            // If Device Exists in Database AND ObservableCollection != Null, Get DB Values and Set Local device TimeOn and TimeOff Values
            if (dbDevice != null && Devices != null)
            {
                // Iterate through ObservableCollection of Devices
                for (int i = 0; i < Devices.Count; i++)
                {

                    // If a device is found with the passed Ip Address, update the (UI)Device.Power value to match the one in the database.
                    if (Devices[i].IpAddress == ipAddress)
                    {
                        Devices[i].Power = dbDevice.Power;
                    }
                }
            }
        }



    }


}
  