using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SmartLight.Models;
//using static Java.Util.Jar.Attributes;

namespace SmartLight;

public class DeviceDB : ContentPage
{

	string _dbPath;

    public string StatusMessage { get; set; }

    private SQLiteConnection conn;

    private void Init()
    {
        if (conn != null)
            return;

        conn = new SQLiteConnection(_dbPath);
        conn.CreateTable<DeviceStorage>();
    }

    public DeviceDB(string dbPath)
    {
        _dbPath = dbPath;
    }

    public void AddNewDevice(string ipAddress, TimeSpan timeOn, TimeSpan timeOff)
    {
        int result = 0;

        try
        {
            Init();

            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new Exception("Ip Address Required");
            } else if (timeOn == TimeSpan.Zero)
            {
                throw new Exception("Time On Value Required");
            } else if (timeOff == TimeSpan.Zero)
            {
                throw new Exception("Time Off Value Required");
            }

            result = conn.Insert(new DeviceStorage { 
                IpAddress = ipAddress,
                TimeOn = timeOn,
                TimeOff = timeOff
                
            }) ;

            StatusMessage = string.Format("{0} record(s) added (IpAddress: {1}, Time On: {2}, Time Off: {3})", result, ipAddress, timeOn, timeOff);
        }

        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to add {0}. Error: {1}", ipAddress, ex.Message);
        }
    }

    public void UpdateDevice(string ipAddress, TimeSpan timeOn, TimeSpan timeOff)
    {
        try
        {
            Init();

            conn.Execute("UPDATE devices SET TimeOn = ?, TimeOff = ? WHERE IpAddress = ?", timeOn, timeOff, ipAddress);

        } catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }

    }

    public DeviceStorage GetDevice(string ipAddress)
    {

        DeviceStorage deviceStorage= null;
        try
        {
            Init();

            var res = conn.Query<DeviceStorage>("SELECT * FROM devices WHERE IpAddress = ?", ipAddress);
            return res.FirstOrDefault();

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            return deviceStorage;
        }
    }

    public List<DeviceStorage> GetAllDevices()
    {
        try
        {
            Init();
            return conn.Table<DeviceStorage>().ToList();
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<DeviceStorage>();
    }

    public DeviceStorage GetByIp(string ipAddress)
    {
        var ip = from i in conn.Table<DeviceStorage>()
                   where i.IpAddress == ipAddress
                   select i;
        return ip.FirstOrDefault();
    }

}