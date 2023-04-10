
#if ANDROID
using Android.Icu.Util;
using System.Net;
#endif
using System.Text;

namespace SmartLight.Models
{
    // Create by Hugo Santana
    // Class Description: This class holds serveral helper methods to assist the application at various stages. Each method is described below.

    internal class Helper
    {
        
        // This method takes in a big endian binary string and converts to little endian form.
        // Input: String (big endian binary string)
        // Output: String (little endian binary string)
        public static string ToLittleEndian(string binString)
        {
            string littleEndian = "";

            for (int i = 8; i < binString.Length + 1; i += 8)
            {
                int lowerPos = i - 8;
                littleEndian = binString.Substring(lowerPos, 8) + littleEndian;
            }

            return littleEndian;
        }

        // This method takes in a binary string and converts to a byte array
        // Input: String (binary string)
        // Output: Byte array
        public static byte[] ToBytes(string binString)
        {
            string input = binString;
            int numOfBytes = input.Length / 8;
            byte[] bytes = new byte[numOfBytes];

            for (int i = 0; i < numOfBytes; ++i)
            {
                bytes[i] = Convert.ToByte(input.Substring(8 * i, 8), 2);
            }
            return bytes;
        }

        // This method takes in a binary string and converts it into a Binary String
        // Input: Byte Array
        // Output: String (binary string)
        public static string ToBinString(byte[] byteArray)
        {

            string binString = "";

            foreach (var bytes in byteArray)
            {
                binString += Convert.ToString(bytes, 2).PadLeft(8, '0');
            }

            return binString;


        }

        // This method takes in a binary string as input and converts the binary to its ASCII equivalent
        // Input: String (binary)
        // Output: String (ASCII representation of binary)
        public static string BinaryToString(string data)
        {
            List<Byte> byteList = new List<Byte>();

            for (int i = 0; i < data.Length; i += 8)
            {
                byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
            }
            return Encoding.ASCII.GetString(byteList.ToArray());

        }

        // This method takes in a Binary String and converts it to the HEX equivalent
        // Input: String (binary string)
        // Output: String (HEX Equivalent of binary string)

        public static string BinaryStringToHexString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                return binary;

            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

            // TODO: check all 1's or 0's... throw otherwise

            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
            {
                // pad to length multiple of 8
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
            }

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }

        // This method takes in a string and converts it to binary.
        // Input: String (ASCII string)
        // Output: String (Binary string)
        public static string StringToBinary(string data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in data.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        // This method takes in a HEX Value string (eg. #FFFFFF) of a colour and outputs the HSV equivalent.
        // Based off www.rags-int-inc.com/phototechstuff/acrcalibration/RGB2HSB.html
        // Input: HEX String
        // Output: HSB Equivalent String
        public static int[] RgbToHsb(string hexValue)
        {
            // Init Vars
            int[] hsvValue = new int[3];
            float hue;

            // Set RGB Values
            int red = int.Parse(hexValue.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            int green = int.Parse(hexValue.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            int blue = int.Parse(hexValue.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);

            // Get Max & Min Values
            int max = Math.Max(red, Math.Max(green, blue));
            int min = Math.Min(red, Math.Min(green, blue));



            //B/V == Largest number divided by 255
            float brightness = (float)max / 255 * 100;

            //Saturation == Difference between the maximum and minimum values, divided by the maximum value
            float saturation = ((float)max - min) / max * 100;

            if (double.IsNaN(saturation))
            {
                saturation = 0;
            }


            if (max == red)
            {
                if ((green - blue == 0) && (max - min == 0))
                {
                    hue = 0;
                }
                else
                {
                    hue = (float)(green - blue) / (float)(max - min);
                }



            }
            else if (max == green)
            {

                if ((blue - red == 0) && (max - min == 0))
                {
                    hue = 0;
                }
                else
                {

                    hue = 2 + (blue - red) / (float)(max - min);
                }
            }
            else
            {

                if ((red - green == 0) && (max - min == 0))
                {
                    hue = 0;
                }
                else
                {

                    hue = 4 + (red - green) / (float)(max - min);
                }
            }


            hue *= 60;

            if (hue < 0)
            {
                hue += 360;
            }

            hsvValue[0] = Convert.ToInt32(hue);
            hsvValue[1] = Convert.ToInt32(saturation);
            hsvValue[2] = Convert.ToInt32(brightness);

            //If Red is max, then Hue = (G - B) / (max - min)
            //If Green is max, then Hue = 2.0 + (B - R) / (max - min)
            //If Blue is max, then Hue = 4.0 + (R - G) / (max - min)

            // Hue value you get needs to be multiplied by 60 to convert it to degrees on the color circle.
            // If Hue becomes negative you need to add 360 to, because a circle has 360 degrees.


            return hsvValue;
        }


        
        public static int UIntHue(int hue)
        {
            hue = ((65535 * hue) / 360) % 65535;

            return hue;
        }

        public static int HueFromUInt(int hue)
        {
            hue = hue * 360 / 65535;

            return hue;
        }

        public static int UIntSB(int sb)
        {
            sb = 65535 * sb / 100;

            return sb;
        }

        public static int SBFromUInt(int sb)
        {
            sb = sb / 65535 * 100;

            return sb;
        }

        // Takes in a device object to obtain current Hue, Saturation, Brightness values for conversion to MAUI Color object
        // Utilises the ColorFromHSV method below
        // Input: Device object
        // Output: Color object
        public static Color DeviceToColor(Device device)
        {
            // Convert LifX Device HSV Values to Int Representation
            int hue = Helper.HueFromUInt(device.Hue);
            int sat = Helper.SBFromUInt(device.Saturation);
            int brightness = Helper.SBFromUInt(device.Brightness);

            // Convert HSV to Color
            Color colorFromHsv = Color.FromHsv(hue, sat, brightness);    
            
            return colorFromHsv;
        }

        // Code obtained from stackoverflow.com/questions/8394178/hsv-rgb-color-space-conversion
        // This method takes in a set of HSV colour values for conversion to a Maui Color object
        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value *= 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromRgba(v, t, p, 255);
            else if (hi == 1)
                return Color.FromRgba(q, v, p, 255);
            else if (hi == 2)
                return Color.FromRgba(p, v, t, 255);
            else if (hi == 3)
                return Color.FromRgba(p, q, v, 255);
            else if (hi == 4)
                return Color.FromRgba(t, p, v, 255);
            else
                return Color.FromRgba(v, p, q, 255);
        }


        public static string GetLabelHelper(string substring)
        {
            // If Substring parses to 0, the label is not set. Null will be returned and the label will eventually be set to the MAC address of the device
            if (decimal.TryParse(substring, out decimal actualNumber) && actualNumber == 0)
            {
                return null;
            }
            else // Else return the ASCII representation of the retreived Binary.
            {
                return BinaryToString(substring);
            }
        }


        public static string GetLocalFilePath(string filename)
        {
            return Path.Combine(FileSystem.AppDataDirectory, filename);
        }

#if ANDROID

        public static Java.Util.Calendar AlarmTime(TimeSpan time)
        {

            // Set Calendar with custom values
            Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            calendar.Set(Java.Util.CalendarField.HourOfDay, time.Hours);
            calendar.Set(Java.Util.CalendarField.Minute, time.Minutes);
            

            return calendar;
        }

#endif






    }
}
