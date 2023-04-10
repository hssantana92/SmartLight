using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Windows.ApplicationModel.VoiceCommands;

namespace SmartLight.Models
{

    // Class Description: This class is used to create objects which 'unpack'/decode the packets received from the LifX bulbs.
    // The Unpack objects are then used to update each 'Device' object.
    public class Unpack
    {
        readonly Helper helper = new();

        // Packet
        private string packet;

        // Frame Header
        private int size;
        private int protocol;
        private int addressable;
        private int tagged;
        private int reserved1;
        private int source;

        // Frame Address
        private string target;
        private string target2;
        private string reserved2;
        private int resRequired;
        private int ackRequired;
        private string reserved3;
        private int sequence;

        // Protocol Header
        private string reserved4;
        private int type;
        private string reserved5;

        // Payload
        private int service;
        private int port;
        private string label;
        private int groupUID;
        private string groupLabel;
        private int updatedAt;
        private int locationUID;
        private int locationUpdatedAt;
        private string locationLabel;
        private int power;
        private int hue;
        private int saturation;
        private int brightness;
        private int kelvin;
        private int vendor;
        private int product;


        public string Packet
        {
            get { return packet; }
            set {
                packet = value;
            }
        }

        // SIZE //
        public int Size
        {
            get { return size; }
            set
            {
                size = value;
            }
        }

        public void SetSize()
        {
            Size = Convert.ToUInt16(Helper.ToLittleEndian(Packet[..16]), 2);
        }

        // PROTOCOL //
        public int Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }

        
        public void SetProtocol()
        {
            string substring = Helper.ToLittleEndian(Packet.Substring(16, 16));
            substring = substring.Substring(4, 12);
            Protocol = Convert.ToUInt16(substring, 2);
        }

        // ADDRESSABLE // 

        public int Addressable
        {
            get { return addressable; }
            set
            {
                addressable = value;
            }
        }

        public void SetAddressable()
        {
            string substring = Helper.ToLittleEndian(Packet.Substring(16, 16));
            substring = substring.Substring(3, 1);
            Addressable = Convert.ToUInt16(substring, 2);
        }

        // TAGGED // 

        public int Tagged
        {
            get { return tagged; }
            set
            {
                tagged = value;
            }
        }

        public void SetTagged()
        {
            string substring = Helper.ToLittleEndian(Packet.Substring(16, 16));
            substring = substring.Substring(2, 1);
            Tagged = Convert.ToUInt16(substring, 2);
        }

        // RESERVED 1 //

        public int Reserved1
        {
            get { return reserved1; }
            set
            {
                reserved1 = value;
            }
        }

        public void SetReserved1()
        {
            string substring = Helper.ToLittleEndian(Packet.Substring(16, 16));
            substring = substring.Substring(0, 2);
            Reserved1 = Convert.ToUInt16(substring, 2);
        }

        // SOURCE //
        public int Source
        {
            get { return source; }
            set
            {
                source = value;
            }
        }

        public void SetSource()
        {
            Source = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(32, 32)), 2);
        }

        // TARGET //

        public string Target
        {
            get { return target; }
            set { target = value; }
        }

        public void SetTarget()
        {
            Target = Packet.Substring(64, 64);
            
        }

        // TARGET 2 //

        public string Target2
        {
            get { return target2; }
            set { target2 = value; }
        }

        public void SetTarget2()
        {
            Target2 = Helper.BinaryStringToHexString(Helper.ToLittleEndian(Packet.Substring(64, 48)));
        }

        // RESERVED 2 // 
        public string Reserved2
        {
            get { return reserved2; }
            set { reserved2 = value; }
        }

        public void SetReserved2()
        {
            Reserved2 = Helper.ToLittleEndian(Packet.Substring(128, 48));
        }

        // RES REQUIRED //

        public int ResRequired
        {
            get { return resRequired; }
            set
            {
                resRequired = value;
            }
        }

        public void SetResRequired()
        {

            ResRequired = Convert.ToUInt16(Packet.Substring(176, 8).Substring(0, 1), 2);
        }

        // ACK REQUIRED // 
        public int AckRequired
        {
            get { return ackRequired; }
            set { ackRequired = value; }
        }

        public void SetAckRequired()
        {
            AckRequired = Convert.ToUInt16(Packet.Substring(176, 8).Substring(1, 1), 2);
        }

        // RESERVED 3 // 
        public string Reserved3
        {
            get { return reserved3; }
            set { reserved3 = value; }
        }

        public void SetReserved3()
        {
            Reserved3 = Helper.ToLittleEndian(Packet.Substring(178, 6));
        }


        // SEQUENCE //

        public int Sequence
        {
            get { return sequence; }
            set
            {
                sequence = value;
            }
        }

        public void SetSequence()
        {
            Sequence = Convert.ToUInt16(Packet.Substring(184, 8), 2);
        }

        // RESERVED 4 //

        public string Reserved4
        {
            get { return reserved4;}
            set
            {
                reserved4 = value;
            }
        }

        public void SetReserved4()
        {
            Reserved4 = Helper.ToLittleEndian(Packet.Substring(192, 64));
        }

        // TYPE //

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public void SetType()
        {
            Type = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(256, 16)), 2);
        }

        // RESERVED 5 //

        public string Reserved5
        {
            get { return reserved5; }
            set { reserved5 = value; }
        }

        public void SetReserved5()
        {
            Reserved5 = Helper.ToLittleEndian(Packet.Substring(272, 16));
        }

        // Set Payloads
        public void SetPayload()
        {
            if (Type == 3) // Broadcast Reply
            {
                Service = Convert.ToUInt16(Packet.Substring(288, 8), 2);
                Port = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(296, 32)), 2);

            }
            else if (Type == 25) // State Label Reply
            {
                // Get Substring of Packet (Label portion)
                string substring = Packet.Substring(288, 256);

                // Set Label
                Label = Helper.GetLabelHelper(substring);

            }
            else if (Type == 53) // State Group Reply
            {
                GroupUID = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(288, 128)), 2);
                GroupLabel = Helper.BinaryToString(Helper.ToLittleEndian(Packet.Substring(416, 256)));
                UpdatedAt = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(672, 64)), 2);

            }
            else if (Type == 50) // State Location Reply
            {
                LocationUID = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(288, 128)), 2);
                LocationLabel = Helper.BinaryToString(Helper.ToLittleEndian(Packet.Substring(416, 256)));
                LocationUpdatedAt = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(672, 64)), 2);

            }
            else if (Type == 22) // State Power Reply
            {
                Power = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(288, 16)), 2);

            }
            else if (Type == 107) // LightState Reply
            {
                Hue = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(288, 16)), 2);
                Saturation = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(304, 16)), 2);
                Brightness = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(320, 16)), 2);
                Kelvin = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(336, 16)), 2);
                // RESERVED 6 Convert.ToUInt16(helper.ToLittleEndian(Packet.Substring(352, 16)), 2);
                Power = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(368, 16)), 2);
                
                // Set Label
                string substring = Packet.Substring(384, 256);
                Label = Helper.GetLabelHelper(substring);

            }
            else if (Type == 33) // State Version Reply
            {

                Vendor = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(288, 32)), 2);
                Product = Convert.ToUInt16(Helper.ToLittleEndian(Packet.Substring(320, 32)), 2);
            }

        }

        public void SetPacket(byte[] bytes)
        {
            Packet = Helper.ToBinString(bytes);
        }

        // This method is a Packet unpacker for basic device information
        // - Target (Mac Address)
        // - Target (Mac Address in Binary Form)
        // - Payload (Basic info: port number, services available)
        public void SetPacketBasic()
        {
            SetTarget();
            SetTarget2();
            SetPayload();

        }

        // This method is a Packet decoder for more detailed device information
        // - 
        public void SetPacketDecode()
        {
            SetSize();
            SetProtocol();
            SetAddressable();
            SetTagged();
            SetReserved1();
            SetSource();
            SetTarget();
            SetTarget2();
            SetReserved2();
            SetResRequired();
            SetAckRequired();
            SetReserved3();
            SetSequence();
            SetReserved4();
            SetType();
            SetReserved5();

            // Set Payload
            SetPayload();


        }



        public int Service
        {
            get { return service; }
            set { service = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        public int GroupUID
        {
            get { return groupUID; }
            set { groupUID = value; }
        }

        public string GroupLabel
        {
            get { return groupLabel; }
            set { groupLabel = value; }
        }

        public int UpdatedAt
        {
            get { return updatedAt; }
            set { updatedAt = value; }
        }

        public int LocationUID
        {
            get { return locationUID; }
            set { locationUID = value; }
        }

        public int LocationUpdatedAt
        {
            get { return locationUpdatedAt; }
            set { locationUpdatedAt = value; }
        }

        public string LocationLabel
        {
            get { return locationLabel; }
            set { locationLabel = value; }
        }

        public int Power
        {
            get { return power; }
            set { power = value; }
        }

        public int Hue
        {
            get { return hue; }
            set { hue = value; }
        }

        public int Saturation
        {
            get { return saturation; }
            set { saturation = value; }
        }

        public int Brightness
        {
            get { return brightness; }
            set { brightness = value; }
        }

        public int Kelvin
        {
            get { return kelvin; }
            set { kelvin = value; }
        }

        public int Vendor
        {
            get { return vendor; }
            set { vendor = value; }
        }

        public int Product
        {
            get { return product; }
            set { product = value; }
        }



        
    }
}
