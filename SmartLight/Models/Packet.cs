//using Android.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartLight.Models
{

    // Class Description: This class allows packets to be created and made ready for transmission to LifX bulbs.
    // The packets have been created according to specifciation at https://lan.developer.lifx.com/docs

    // This class also holds several methods to create specific get and set packets to retreive information from the bulbs
    // and set bulb information.

    internal class Packet
    {
        readonly Helper helper = new();
        private byte[] fullPacket;

        // Frame Header
        private string frameHeader;
        private string size;
        private string protocol;
        private string addressable;
        private string tagged;
        private string origin;
        private string source;

        // Frame Address
        private string frameAddress;
        private string target;
        private string reserved48;
        private string resRequired;
        private string ackRequired;
        private string reserved6;
        private string sequence;

        // Protocol Header
        private string protocolHeader;
        private string reserved64;
        private string type;
        private string reserved16;

        // Payloads
        private string payload;
        private string level;
        private string label;
        private string hue;
        private string saturation;
        private string brightness;
        private string kelvin;
        private string duration;

        // Getters and Setters
        public byte[] FullPacket { get { return fullPacket; } set { fullPacket = value; } }
        public string FrameHeader
        {
            get { return frameHeader; }
            set { frameHeader = value; }
        }

        public string Size
        {
            get { return size; }
            set { size = Convert.ToString(UInt16.Parse(value), 2).PadLeft(16, '0'); }
        }
        public string Protocol
        {
            get { return protocol; }
            set { protocol = Convert.ToString(UInt16.Parse(value), 2).PadLeft(12, '0'); }
        }
        public string Addressable
        {
            get { return addressable; }
            set { addressable = Convert.ToString(UInt16.Parse(value), 2).PadLeft(1, '0'); }
        }

        public string Tagged
        {
            get { return tagged; }
            set { tagged = Convert.ToString(UInt16.Parse(value), 2).PadLeft(1, '0'); }
        }

        public string Origin
        {
            get { return origin; }
            set { origin = Convert.ToString(UInt16.Parse(value), 2).PadLeft(2, '0'); }
        }
        public string Source
        {
            get { return source; }
            set
            {
                source = Convert.ToString(UInt16.Parse(value), 2).PadLeft(32, '0');
            }
        }

        public string FrameAddress
        {
            get { return frameAddress; }
            set { frameAddress = value; }
        }

        public string Target
        {
            get { return target; }
            set { 
                if (value == "0")
                {
                    target = Convert.ToString(UInt16.Parse(value), 2).PadLeft(64, '0');
                } else
                {
                    target = value;
                }

            }
        }
        public string Reserved48
        {
            get { return reserved48; }
            set { reserved48 = Convert.ToString(UInt16.Parse(value), 2).PadLeft(48, '0'); }
        }

        public string ResRequired
        {
            get { return resRequired; }
            set { resRequired = Convert.ToString(UInt16.Parse(value), 2).PadLeft(1, '0'); }
        }

        public string AckRequired
        {
            get { return ackRequired; }
            set { ackRequired = Convert.ToString(UInt16.Parse(value), 2).PadLeft(1, '0'); }
        }

        public string Reserved6
        {
            get { return reserved6; }
            set
            {
                reserved6 = Convert.ToString(UInt16.Parse(value), 2).PadLeft(6, '0');
            }
        }

        public string Sequence
        {
            get { return sequence; }
            set
            {
                sequence = Convert.ToString(UInt16.Parse(value), 2).PadLeft(8, '0');
            }
        }

        public string ProtocolHeader
        {
            get { return protocolHeader; }
            set { protocolHeader = value; }
        }

        public string Reserved64
        {
            get { return reserved64; }
            set
            {
                reserved64 = Convert.ToString(UInt16.Parse(value), 2).PadLeft(64, '0');
            }
        }

        public string Type
        {
            get { return type; }
            set { type = Convert.ToString(UInt16.Parse(value), 2).PadLeft(16, '0'); }
        }

        public string Reserved16
        {
            get { return reserved16; }
            set { reserved16 = Convert.ToString(UInt16.Parse(value), 2).PadLeft(16, '0'); }
        }

        public string Payload
        {
            get { return payload; }
            set { payload = value; }
        }

        public string Level
        {
            get { return level; }
            set { level = Convert.ToString(UInt16.Parse(value), 2).PadLeft(16, '0'); }
        }

        public string Label
        {
            get { return label; }
            set { label = Helper.StringToBinary(value).PadLeft(256, '0'); }
        }

        public string Hue
        {
            get { return hue; }
            set { hue = Convert.ToString(UInt16.Parse(value), 2).PadLeft(16, '0'); }
        }

        public string Saturation
        {
            get { return saturation; }
            set { saturation = Convert.ToString(UInt16.Parse(value), 2).PadLeft(16, '0'); }
        }

        public string Brightness
        {
            get { return brightness; }
            set { brightness = Convert.ToString(UInt16.Parse(value), 2).PadLeft(16, '0'); }
        }

        public string Kelvin
        {
            get { return kelvin; }
            set { kelvin = Convert.ToString(UInt16.Parse(value), 2).PadLeft(16, '0'); }
        }

        public string Duration
        {
            get { return duration; }
            set { duration = Convert.ToString(UInt16.Parse(value), 2).PadLeft(32, '0'); }
        }

        
        // Helpers

        public void SetPacket()
        {
            // Set Packet Headers
            FrameHeader = Helper.ToLittleEndian(Size) + Helper.ToLittleEndian(Origin + Tagged + Addressable + Protocol) + Helper.ToLittleEndian(Source);

            FrameAddress = Target + Helper.ToLittleEndian(Reserved48) + Reserved6 + AckRequired + ResRequired + Helper.ToLittleEndian(Sequence);

            ProtocolHeader = Reserved64 + Helper.ToLittleEndian(Type) + Reserved16;

            

            // Concatentate packet headers and transform into transferrable message
            if (Payload != null)
            {
                FullPacket = Helper.ToBytes(FrameHeader + FrameAddress + ProtocolHeader + Payload);

            } else
            {
                FullPacket = Helper.ToBytes(FrameHeader + FrameAddress + ProtocolHeader);
            }

        }

        public void GetLabelPacket(string target)
        {
            Size = "36";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "0";
            Origin = "0";
            Source = "2";
            Target = target;
            Reserved48 = "0";
            ResRequired = "0";
            AckRequired = "0";
            Reserved6 = "0";
            Sequence = "0";
            Reserved64 = "0";
            Type = "23";
            Reserved16 = "0";

            SetPacket();
        }

        public void GetPowerPacket(string target)
        {
            Size = "36";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "0";
            Origin = "0";
            Source = "2";
            Target = target;
            Reserved48 = "0";
            ResRequired = "0";
            AckRequired = "0";
            Reserved6 = "0";
            Sequence = "0";
            Reserved64 = "0";
            Type = "20";
            Reserved16 = "0";

            SetPacket();
        }

        public void SetBroadcastPacket()
        {
            Size = "36";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "1";
            Origin = "0";
            Source = "2";
            Target = "0";
            Reserved48 = "0";
            ResRequired = "0";
            AckRequired = "0";
            Reserved6 = "0";
            Sequence = "1";
            Reserved64 = "0";
            Type = "2";
            Reserved16 = "0";

            SetPacket();
        }

        public void GetLabelBroadcastPacket()
        {
            Size = "36";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "1";
            Origin = "0";
            Source = "2";
            Target = "0";
            Reserved48 = "0";
            ResRequired = "1";
            AckRequired = "0";
            Reserved6 = "0";
            Sequence = "0";
            Reserved64 = "0";
            Type = "23";
            Reserved16 = "0";

            SetPacket();
        }

        public void GetStateBroadcastPacket()
        {
            Size = "36";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "1";
            Origin = "0";
            Source = "2";
            Target = "0";
            Reserved48 = "0";
            ResRequired = "1";
            AckRequired = "0";
            Reserved6 = "0";
            Sequence = "0";
            Reserved64 = "0";
            Type = "101";
            Reserved16 = "0";

            SetPacket();
        }

        public void GetProductBroadcastPacket()
        {
            Size = "36";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "1";
            Origin = "0";
            Source = "2";
            Target = "0";
            Reserved48 = "0";
            ResRequired = "1";
            AckRequired = "0";
            Reserved6 = "0";
            Sequence = "0";
            Reserved64 = "0";
            Type = "32";
            Reserved16 = "0";

            SetPacket();
        }

        public void GetPowerBroadcastPacket()
        {
            Size = "36";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "1";
            Origin = "0";
            Source = "2";
            Target = "0";
            Reserved48 = "0";
            ResRequired = "1";
            AckRequired = "0";
            Reserved6 = "0";
            Sequence = "0";
            Reserved64 = "0";
            Type = "20";
            Reserved16 = "0";

            SetPacket();
        }

        public void SetPowerPacket(string target, string value)
        {
            // Set packet values
            Size = "38";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "0";
            Origin = "0";
            Source = "2";
            Target = target;
            Reserved48 = "0";
            ResRequired = "1";
            AckRequired = "1";
            Reserved6 = "0";
            Sequence = "0";
            Reserved64 = "0";
            Type = "21";
            Reserved16 = "0";
            Level = value;

            // Set Payload
            Payload = Helper.ToLittleEndian(Level);

            // Set packet for transmission
            SetPacket();
        }

        public void SetLabelPacket(string target, string value)
        {
            // Set packet values
            Size = "68";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "0";
            Origin = "0";
            Source = "5";
            Target = target;
            Reserved48 = "0";
            ResRequired = "1";
            AckRequired = "1";
            Reserved6 = "0";
            Sequence = "0";
            Reserved64 = "0";
            Type = "24";
            Reserved16 = "0";
            Label = value;

            // Set Payload
            Payload = Label;

            // Set packet for transmission
            SetPacket();
        }

        public void SetColourPacket(string target, string hue, string saturation, string brightness, string kelvin, string duration)
        {
            // Set packet values
            Size = "43";
            Protocol = "1024";
            Addressable = "1";
            Tagged = "0";
            Origin = "0";
            Source = "5";
            Target = target;
            Reserved48 = "0";
            ResRequired = "1";
            AckRequired = "1";
            Reserved6 = "0";
            Sequence = "0";
            Reserved64 = "0";
            Type = "102";
            Reserved16 = "0";
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
            Kelvin = kelvin;
            Duration = duration;


            // Set Payload
            Payload = "00000000" + Helper.ToLittleEndian(Hue) + Helper.ToLittleEndian(Saturation) + Helper.ToLittleEndian(Brightness) + Helper.ToLittleEndian(Kelvin) + Helper.ToLittleEndian(Duration);

            // Set packet for transmission
            SetPacket();
        }




    }
}
