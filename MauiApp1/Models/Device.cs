//using Android.Service.Chooser;
//using Android.Mtp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartLight.Models
{


    // Device Class Description:
    // This class permits multiple device objects to be created which holds all necessary information relating to the LifX bulbs.

    // The class extends the ObservableObject (as part of the MVVM Community Toolkit) class to allow GUI updates to occur as the objects are modified
    // avoiding the need to manually execute INotifyPropertyChanged events.

    public partial class Device : ObservableObject
    {
        
        [ObservableProperty]
        private string ipAddress;

        [ObservableProperty]
        public string targetBin;

        
        [ObservableProperty]
        public string targetMac;

        [ObservableProperty]
        public string service;

        [ObservableProperty]
        public string port;

        [ObservableProperty]
        public string sequence;

        [ObservableProperty]
        public string label;
        
        [ObservableProperty]
        public string groupUID;

        [ObservableProperty]
        public string groupLabel;

        [ObservableProperty]
        public string updatedAt;

        [ObservableProperty]
        public string locationUID;

        [ObservableProperty]
        public string locationUpdatedAt;

        [ObservableProperty]
        public string locationLabel;

        [ObservableProperty]
        public bool power;

        [ObservableProperty]
        public int hue;

        [ObservableProperty]
        public int saturation;

        [ObservableProperty]
        public int brightness;

        [ObservableProperty]
        public int kelvin;

        [ObservableProperty]
        public int vendor;

        [ObservableProperty]
        public int product;

        [ObservableProperty]
        public bool colour;

        [ObservableProperty]
        public TimeSpan timeOff;

        [ObservableProperty]
        public TimeSpan timeOn;



    }

}
