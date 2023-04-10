using System.Net.Sockets;
using SmartLight.Models;
// Remove Device ambiguity
using Device = SmartLight.Models.Device;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace SmartLight.Services
{
    public class DeviceService
    {

        // The DiscoverDevices method performs all necessary functions to Search for devices and return an Observable Collection of Device objects.
        // The method creates various broadcast packets to retreive all necessary bulb/device info. The packets obtained are then used to create device objects
        // which can then be used to perform various tasks on.
        public static async Task<ObservableCollection<Device>> DiscoverDevices()
        {

            // Create Broadcast Packet
            Packet broadcastPacket = new();
            broadcastPacket.SetBroadcastPacket();


            // Create Get State Packet
            // This Packet retrieves more detailed information such as the bulb Colour, Brightness Level, Power Status, & Device name/label.
            Packet broadcastGetState = new();
            broadcastGetState.GetStateBroadcastPacket();


            // Create Get Product Packet
            // This packet retrieves bulb specific information 
            Packet broadcastGetProduct = new();
            broadcastGetProduct.GetProductBroadcastPacket();

            // Create Device List
            ObservableCollection<Device> devices = new();


            // Send Packet and receive responses into dictionary
            Task<Dictionary<string, byte[]>> broadcastRec = SendPacket(broadcastPacket.FullPacket, "255.255.255.255");
            Task<Dictionary<string, byte[]>> broadcastState = SendPacket(broadcastGetState.FullPacket, "255.255.255.255");
            Task<Dictionary<string, byte[]>> broadcastProduct = SendPacket(broadcastGetProduct.FullPacket, "255.255.255.255");

            Dictionary<string, byte[]> broadcastRecAwait = await broadcastRec;
            Dictionary<string, byte[]> broadcastStateAwait = await broadcastState;

            Thread.Sleep(500);
            Dictionary<string, byte[]> broadcastProductAwait = await broadcastProduct;

            // Sort devices for consistent view
            var broadcastRecAwaitSorted = new SortedDictionary<string, byte[]>(broadcastRecAwait);

            // If data received create
            foreach (KeyValuePair<string, byte[]> br in broadcastRecAwaitSorted)
            {
                // Create tmp device
                Device tmpDevice = new();

                // Decode packet dictionary and set basic device info 
                // i.e. mac address, ip address, port num.
                SetDevice(ref tmpDevice, br.Key, br.Value, ref broadcastStateAwait, ref broadcastProductAwait);

                // Add tmpDevice to devices list
                devices.Add(tmpDevice);

            }

            return devices;
        }

        // TogglePower receives a device object, figures out if the power is currently on or off and then creates a packet to turn the bulb on/off

        public static void TogglePower(Device device)
        {

            string value;
            value = device.Power ? "65535" : "0";

            // Create Power Packet
            Packet powerPacket = new();
            powerPacket.SetPowerPacket(device.TargetBin, value);

            // Broadcast Packet
            BroadcastPackets(powerPacket.FullPacket, device.IpAddress);

        }

        // Method to send packets when response IS required
        public static async Task<Dictionary<string, byte[]>> SendPacket(byte[] pkt, string ipAddress)
        {
            Thread.Sleep(100);
            var Client = new UdpClient();
            bool packetReceived = true;

            // Init Dictionary List
            Dictionary<string, byte[]> devices = new();

            // Send packet
            Client.Send(pkt, pkt.Length, ipAddress, 56700);

            if (ipAddress == "255.255.255.255")
            {
                Client.EnableBroadcast = true;
            }

            while (packetReceived)
            {
                
                try
                {
                    // Creates Async Task to retrieve data from device.
                    var receiveAsync = Client.ReceiveAsync();

                    // Receive packet or timeout after .5 second (i.e end of stream).
                    if (await Task.WhenAny(receiveAsync, Task.Delay(1000)) == receiveAsync)
                    {

                        // Set response packet byte array
                        byte[] data = receiveAsync.Result.Buffer;

                        // Set response device IP address
                        var serverEp = receiveAsync.Result.RemoteEndPoint;

                        // Add to devices Dictionary if ip/device doesn't already exist in dictionary
                        if (!devices.ContainsKey(serverEp.Address.ToString()))
                        {
                            devices.Add(serverEp.Address.ToString(), data);
                        }


                    } 
                    else // end of stream. 
                    {
                        packetReceived = false;
                        break;
                    }


                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    break;
                }


            }

            // Close socket and return dictionary of packets
            Client.Close();
            return devices;
        }


        // Method to send packets when NO response is required
        public static void BroadcastPackets(byte[] pkt, string ipAddress)
        {

            var Client = new UdpClient
            {
                EnableBroadcast = true
                
            };

            // Send packet
            Client.Send(pkt, pkt.Length, ipAddress, 56700);

            // Close socket
            Client.Close();
            
        }


        // Method Description: This method receives a reference to a temporary device object as well as an ip address
        // and response bytes to a device discovery packet.
        // Inputs: Reference to device, ip address, bytes
        // Outputs: Nil. Updates device object by reference.

        public static void SetDevice(ref Device device, string ipaddress, byte[] bytes, ref Dictionary<string, byte[]> stateValues, ref Dictionary<string, byte[]> productInfo)
        {

            // Check DB to see if IpAddress exists for Auto Timer
            DeviceStorage dbDevice = App.DeviceDataBase.GetDevice(ipaddress);

            // If Device Exists in Database, Get DB Values and Set Local device TimeOn and TimeOff Values
            if (dbDevice != null)
            {
                device.TimeOn = dbDevice.TimeOn;
                device.TimeOff = dbDevice.TimeOff;
            }

            Unpack decoder = new();

            // Set packet decoder as a basic device info decoder packet.
            decoder.SetPacket(bytes);
            decoder.SetPacketBasic();

            // Set Device Ip Address
            device.IpAddress = ipaddress;

            // Use decoder packet to set local device (temporary device) basic information for later addition to the observable collection.
            DecoderBasicInfoHelper(ref device, ref decoder);

            // Reset packet decoder as a state value decoder packet
            decoder.SetPacket(stateValues[ipaddress]);
            decoder.SetPacketDecode();

            // Use decoder packet to set local device advanced information for later addition to the observable collection
            DecoderAdvancedInfoHelper(ref device, ref decoder);

            // Reset packet decoder as a product info decoder packet
            decoder.SetPacket(productInfo[ipaddress]);
            decoder.SetPacketDecode();

            // Use decoder packet to set local device product information for later addition to the observable collection
            DecoderProductInfoHelper(ref device, ref decoder);

            // Use device reference to set the colour capability of the bulb.
            DecoderColourInfoHelper(ref device);

        }


        // DecoderBasicInfoHelper Method Description

        // Input: Device reference, Unpack object reference
        // Output: Updates device object with basic device information (Mac Address, Sequence #, Port #)
        public static void DecoderBasicInfoHelper(ref Device device, ref Unpack packet)
        {
            device.TargetMac = packet.Target2;
            device.TargetBin = packet.Target;
            device.Sequence = packet.Sequence.ToString();
            device.Port = packet.Port.ToString();
        }


        // DecoderAdvancedInfoHelper Method Description

        // Input: Device reference, Unpack object reference
        // Output: Updates device object with advanced device information (Label, Power Status, Brightness, Colour)
        public static void DecoderAdvancedInfoHelper(ref Device device, ref Unpack packet)
        {

            // If Label is null or empty set Mac Address as Label
            if (String.IsNullOrEmpty(packet.Label))
            {
                packet.Label = device.TargetMac;
            }

            device.Label = packet.Label;
            device.Power = packet.Power != 0;
            device.Brightness = packet.Brightness;
            device.Hue = packet.Hue;
            device.Kelvin = packet.Kelvin;
            device.Saturation = packet.Saturation;

        }

        // DecoderProductInfoHelper Method Description

        // Input: Device reference, Unpack object reference
        // Output: Updates device object with device product information (Vendor (LifX), Product #)
        public static void DecoderProductInfoHelper(ref Device device, ref Unpack packet)
        {
            device.Vendor = packet.Vendor;
            device.Product = packet.Product;
        }


        // DecoderColourInfoHelper Method Description

        // Input: Device reference
        // Output: Updates the device object with the type of LifX bulb it is ie. the color capability of the light.
        public static void DecoderColourInfoHelper(ref Device device)
        {
            // Create product number arrays, more products exist but are not included in this app at the moment.
            // This app currently only supports White, Warm to White, and Colour devices.
            // Product info can be found at https://lan.developer.lifx.com/docs/product-registry
            
            // White - These product #s only has one shade of white
            int[] White = { 51, 61, 66, 82, 85, 87, 88, 100, 101 };

            // Warm to White - These product #s have no colour but support colour temperature control
            int[] WarmToWhite = { 10, 11, 18, 19, 39, 50, 60, 81, 96, 113, 114 };

            if (WarmToWhite.Contains(device.Product))
            {
                device.Colour = false;
            }
            else if (White.Contains(device.Product))
            {
                device.Colour = false;
            }
            else // If product # not in either array we assume it is a colour supporting device for the purpose of this app.
            {
                device.Colour = true;
            }

        }


    }



}




