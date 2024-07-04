using System.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;
using iRSDKSharp;
using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using Hessburg;
using TzLookup;
using NodaTime;
using NodaTime.TimeZones;


public class iracingConnector : MonoBehaviour
{
    public SdkWrapper wrapper;
    public SunLight SunLight;
    public float workingTime = 0;
    public GameObject simRig;
    void Start()
    {
        wrapper = new SdkWrapper();
        wrapper.EventRaiseType = SdkWrapper.EventRaiseTypes.CurrentThread;
        
            // Only update telemetry 10 times per second
        wrapper.TelemetryUpdateFrequency = 10;

            // Attach some useful events so you can respond when they get raised
        wrapper.Connected += wrapper_Connected;
        wrapper.Disconnected += wrapper_Disconnected;
        wrapper.SessionInfoUpdated += wrapper_SessionInfoUpdated;
        wrapper.TelemetryUpdated += wrapper_TelemetryUpdated;
        SunLight.latitude = 0.0f; // Set latitude (float, -90.0 to 90.0)
        SunLight.longitude = 0.0f; // Set longitude (float -180.0 to 180.0) // Timezone / Offset from UTC in hours (float -12.0 to 14.0)
        SunLight.dayOfYear = 183; // The day of the year (1 to 355 (366 in leap years)
        SunLight.timeInHours = 12; // Time in decimal hours – 8.5 equals 12:30 PM (float 0.0 to 24.0)
        SunLight.progressTime = false; // Time will progress automatically (boolean)
        SunLight.timeProgressFactor = 1.0f; // How fast time will progress, real time would be 1.0 (float 0.0 – )
        wrapper.Start();
        UnityEngine.Debug.Log("iRSDK Status: Started!");
        if (simRig == null)
            simRig = GameObject.FindWithTag("SimRig");
        DisableSunLight();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StatusChanged()
    {
        if (wrapper.IsConnected)
        {
            if (wrapper.IsRunning)
            {

                UnityEngine.Debug.Log("Status: connected!");
            }
            else
            {
                UnityEngine.Debug.Log("Status: disconnected.");
                DisableSunLight();
            }
        }
        else
        {
            if (wrapper.IsRunning)
            {
                UnityEngine.Debug.Log("Status: disconnected, waiting for sim...");
                DisableSunLight();
            }
            else
            {
                UnityEngine.Debug.Log("Status: disconnected");
                DisableSunLight();
            }
        }
    }

    private void wrapper_Connected(object sender, EventArgs e)
        {
            this.StatusChanged();
        }

    private void wrapper_Disconnected(object sender, EventArgs e)
        {
            this.StatusChanged();
        }

    private void wrapper_SessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {
            //Get variables from iRacing SDK
            var irlatitude = e.SessionInfo["WeekendInfo"]["TrackLatitude"].GetValue();
            var irlongitude = e.SessionInfo["WeekendInfo"]["TrackLongitude"].GetValue();
            var irdate = e.SessionInfo["WeekendInfo"]["WeekendOptions"]["Date"].GetValue();
            var irhour = e.SessionInfo["WeekendInfo"]["WeekendOptions"]["TimeOfDay"].GetValue();

            //Format variables to not suck
            irhour = irhour.Replace("-", ":"); 
            var dateTime = irdate + ", " + irhour;
            
            //Create a date time. We could just use Nodatime and centralize this. But I am dumb.
            System.DateTime irDateTime = System.DateTime.Parse(dateTime);
            
            //Set Longitude and latitude
            double latitude = 0.0f;
            double longitude = 0.0f;
            irlatitude = irlatitude.Substring(0, irlatitude.Length - 2);
            latitude = Convert.ToDouble(irlatitude);
            SunLight.latitude = (float)latitude;

            irlongitude = irlongitude.Substring(0, irlongitude.Length - 2);
            longitude = Convert.ToDouble(irlongitude);
            SunLight.longitude = (float)longitude;

            //Get the timezone
            var timeZoneName = TimeZoneLookup.Iana((float)latitude, (float)longitude);
            
            //Figure out the UTC Offset of the timezone. (Including if it has daylights savings time)
            var provider = DateTimeZoneProviders.Tzdb;
            DateTimeZone currentDTZ = provider[timeZoneName];
            int irDay = irDateTime.Day;
            int irMonth = irDateTime.Month;
            int irYear = irDateTime.Year;
            ZoneInterval interval = currentDTZ.GetZoneInterval(Instant.FromUtc(irYear, irMonth, irDay, 0, 0));
            
            //Set the UTC time
            var nonoffset = interval.WallOffset.ToString();
            double irOffset = Convert.ToDouble(nonoffset);
            SunLight.offsetUTC = (float)irOffset;
            UnityEngine.Debug.Log(e.SessionInfo["WeekendInfo"]["WeekendOptions"]["Date"].GetValue());
            SunLight.dayOfYear = irDateTime.DayOfYear;
    }

    private void DisableSunLight()
    {
        //Disables SunLight connection to iRacing, but enables local timezone
            //Sets latitude and longitude (Needs to be automated somehow?)
            double latitude = 39f;
            double longitude = 104f;
            //Get the timezone
            var timeZoneName = TimeZoneLookup.Iana((float)latitude, (float)longitude);
            
            //Figure out the UTC Offset of the timezone. (Including if it has daylights savings time)
            var provider = DateTimeZoneProviders.Tzdb;
            DateTimeZone currentDTZ = provider[timeZoneName];
            int irDay = DateTime.Now.Day;
            int irMonth = DateTime.Now.Month;
            int irYear = DateTime.Now.Year;
            ZoneInterval interval = currentDTZ.GetZoneInterval(Instant.FromUtc(irYear, irMonth, irDay, 0, 0));
            
            //Set the UTC time
            var nonoffset = interval.WallOffset.ToString();
            double irOffset = Convert.ToDouble(nonoffset);
            SunLight.offsetUTC = (float)irOffset;

        double currentHour = DateTime.Now.TimeOfDay.TotalHours;
        SunLight.timeInHours = (float)currentHour; // Time in decimal hours – 8.5 equals 12:30 PM (float 0.0 to 24.0)
        UnityEngine.Debug.Log("Current Time: " + SunLight.timeInHours);
        SunLight.progressTime = false; // Time will progress automatically (boolean)
        SunLight.longitude = (float)longitude;
        SunLight.latitude = (float)latitude;
        workingTime = 0f;
        SunLight.dayOfYear = System.DateTime.Now.DayOfYear;
        SunLight.northDirection = 0f;
    }

    private void wrapper_TelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
    {
        // Get the current SessionTimeOfDay (lets us keep in sync with iRacing 100% of the time)
        float simRotOffset = simRig.transform.eulerAngles.y;
        var sessionSecondOfDay = wrapper.GetTelemetryValue<float>("SessionTimeOfDay");
        var sessionTimeOfDay = (sessionSecondOfDay.Value / 3600); //Convert to a time of day
        var irYawNorth = wrapper.GetTelemetryValue<float>("YawNorth");
        var degYawNorth = (irYawNorth.Value * Mathf.Rad2Deg);
        degYawNorth = (degYawNorth + 180f - simRotOffset);
        float currentYawNorth = Mathf.Repeat(degYawNorth, 360f);



        SunLight.northDirection = 360f - currentYawNorth;

        if (sessionTimeOfDay > (0.01f + workingTime))
        {
            SunLight.timeInHours = sessionTimeOfDay;
            workingTime = sessionTimeOfDay;
        }

        if (sessionTimeOfDay < workingTime)
        {
            workingTime = 0;
        }

    }

    void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("Application ending after " + Time.time + " seconds");
        wrapper.Stop();
    }       
}
