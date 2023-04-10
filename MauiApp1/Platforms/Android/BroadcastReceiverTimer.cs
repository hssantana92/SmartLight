using Android.App;
using Android.Content;
using Android.App.Job;
using Android.Widget;
using Microsoft.Maui.Controls;
using Device = SmartLight.Models.Device;
using SmartLight.Services;
using SmartLight.Models;
using SmartLight.ViewModel;
using CommunityToolkit.Mvvm.Messaging;
using SmartLight.Messages;

namespace SmartLight.Platforms.Android
{
    [BroadcastReceiver]
    public class BroadcastReceiverTimer : BroadcastReceiver
    {

        public override void OnReceive(Context context, Intent intent)
        {
            // Show Toast
            Toast.MakeText(context, "Toggling power for " + intent.GetStringExtra("Label"), ToastLength.Short).Show();

            // Create Device
            Device receiverDevice = new Device();
            receiverDevice.IpAddress = intent.GetStringExtra("IP");
            receiverDevice.TargetBin = intent.GetStringExtra("TargetBin");
            receiverDevice.Label = intent.GetStringExtra("Label");

            // Will turn ON device if power set to TRUE 
            // TO DO: Create seperate Alarms for ON TASK and OFF TASK
            receiverDevice.Power = true;


            // Toggle Light Power
            DeviceService.TogglePower(receiverDevice);

            // Update View
            WeakReferenceMessenger.Default.Send(new UpdateDevice(receiverDevice.IpAddress));

            // SET NEW ALARM 24 HOURS IN ADVANCE

            // Intent
            Intent timerIntent = new Intent(Platform.CurrentActivity, typeof(BroadcastReceiverTimer));
            timerIntent.PutExtra("IP", receiverDevice.IpAddress);
            timerIntent.PutExtra("TargetBin", receiverDevice.TargetBin);
            timerIntent.PutExtra("Label", receiverDevice.Label);

            // Pending Intent
            PendingIntent timerPendingIntent = PendingIntent.GetBroadcast(Platform.CurrentActivity, 0, timerIntent, 0);

            // Alarm Manager
            AlarmManager alarmManager = (AlarmManager)Platform.CurrentActivity.GetSystemService(Context.AlarmService);

            // Get Current Time
            Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();



            // Set Alarm for 24 hours in advance
            alarmManager.SetExact(AlarmType.RtcWakeup, calendar.TimeInMillis + 86400000, timerPendingIntent);


        }

     
    }


    public class TimerAlarm
    {
        public void SetTimer(Device device)
        {

            Intent timerIntent = new Intent(Platform.CurrentActivity,typeof(BroadcastReceiverTimer));
            timerIntent.PutExtra("IP", device.IpAddress);
            timerIntent.PutExtra("TargetBin", device.TargetBin);
            timerIntent.PutExtra("Label", device.Label);

            PendingIntent timerPendingIntent = PendingIntent.GetBroadcast(Platform.CurrentActivity, 0, timerIntent, 0);

            // Init Alarm Manager
            AlarmManager alarmManager = (AlarmManager)Platform.CurrentActivity.GetSystemService(Context.AlarmService);

            // Clear all previous timers
            alarmManager.Cancel(timerPendingIntent);


            // Set Calendar Times for device TimeSpan values
            Java.Util.Calendar calendarOn = Helper.AlarmTime(device.TimeOn);
            System.Diagnostics.Debug.WriteLine(calendarOn.TimeInMillis);

           
            //Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            //calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            alarmManager.SetExact(AlarmType.RtcWakeup, calendarOn.TimeInMillis, timerPendingIntent);


            Toast.MakeText(Platform.CurrentActivity, "Alarm Set for " + device.Label, ToastLength.Short).Show();


        }
    }

}
