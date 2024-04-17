using System.Timers;
using System;
using System.Management;


namespace SalatTimeWidgetApp.Models
{


    public class BatteryPercentageProvider
    {
        public float GetBatteryPercentage()
        {
            float batteryPercentage = 0.0f;
            ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Battery");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    batteryPercentage = Convert.ToInt32(queryObj["EstimatedChargeRemaining"]);
                    string batteryPercentageString = $"Battery Percentage: {batteryPercentage}%";
                    Console.WriteLine(batteryPercentageString);

                }
            }
            return batteryPercentage;
        }
    }
}