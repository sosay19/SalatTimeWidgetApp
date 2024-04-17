using Newtonsoft.Json;
using SalatTimeWidgetApp.Models;
using SalatTimeWidgetApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace SalatTimeWidgetApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _currentBatteryPercentage;
        public string CurrentBatteryPercentage
        {
            get => _currentBatteryPercentage;
            set
            {
                _currentBatteryPercentage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentBatteryPercentage"));
            }
        }

        public MainViewModel()
        {
            BatteryPercentageProvider batteryPercentageProvider = new BatteryPercentageProvider();
            Console.WriteLine(batteryPercentageProvider.GetBatteryPercentage());
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += async (sender, args) =>
            {
                CurrentTime = DateTime.Now.ToString("HH:mm:ss - dd ddd MMM");
                CurrentBatteryPercentage = "Battery percentage: " + batteryPercentageProvider.GetBatteryPercentage().ToString() + "%";
                // For debugging: Output the time of the timer tick
                Console.WriteLine("Timer Tick: " + CurrentTime);

                // Call UpdateSalatTimesAsync and set NextPrayer and SalatTimes when it's complete
                await UpdateSalatTimesAsync("Brussels", "Belgium", 9);
            };
            timer.Start();

        }

        private ObservableCollection<SalatTimeItem> _salatTimes = new ObservableCollection<SalatTimeItem>();
        public ObservableCollection<SalatTimeItem> SalatTimes
        {
            get => _salatTimes;
            set
            {
                _salatTimes = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SalatTimes"));
            }
        }
        public async Task UpdateSalatTimesAsync(string city, string country, int method)
        {
            try
            {
                var salatTimeService = new SalatTimeService();
                var salatTimingsJson = await salatTimeService.GetSalatTimingsAsync(city, country, method);

                // For debugging: Output the received JSON
                Console.WriteLine("Received JSON: " + salatTimingsJson);

                // For debugging: Attempt to parse JSON
                var extractedSalatTimes = ParseSalatTimingsFromJson(salatTimingsJson);

                // For debugging: Output the extracted Salat times
                foreach (var item in extractedSalatTimes)
                {
                    Console.WriteLine("Salat: " + item.SalatName + " Time: " + item.SalatTime);
                }

                // Set NextPrayer and SalatTimes
                if (extractedSalatTimes.Count > 0)
                {
                    NextPrayer = extractedSalatTimes[0].SalatTime; // Example: Set the next prayer
                    SalatTimes.Clear();
                    foreach (var item in extractedSalatTimes)
                    {
                        SalatTimes.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        private List<SalatTimeItem> ParseSalatTimingsFromJson(string json)
        {
            try
            {
                var deserializedData = JsonConvert.DeserializeObject<SalatTimingsData>(json);

                // Assuming the JSON structure has a 'timings' property containing Salat times
                var timings = deserializedData.data.timings;

                var salatTimes = new List<SalatTimeItem>
        {
            new SalatTimeItem { SalatName = "Fajr", SalatTime = timings.Fajr },
            new SalatTimeItem { SalatName = "Dhuhr", SalatTime = timings.Dhuhr },
            new SalatTimeItem { SalatName = "Asr", SalatTime = timings.Asr },
            new SalatTimeItem { SalatName = "Maghrib", SalatTime = timings.Maghrib },
            new SalatTimeItem { SalatName = "Isha", SalatTime = timings.Isha }
        };

                return salatTimes;
            }
            catch (Exception ex)
            {
                // Handle JSON parsing errors here
                Console.WriteLine("Error parsing JSON: " + ex.Message);
                return new List<SalatTimeItem>();
            }
        }


        private string _currentTime;
        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentTime"));
            }
        }
 
        private string _nextPrayer;
        public string NextPrayer
        {
            get => _nextPrayer;
            set
            {
                _nextPrayer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NextPrayer"));
            }
        }
    }
}
