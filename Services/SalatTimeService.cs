using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace SalatTimeWidgetApp.Services
{
  
    public class SalatTimeService
    {
        private readonly HttpClient _httpClient;

        public SalatTimeService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetSalatTimingsAsync(string city, string country, int method)
        {
            string apiUrl = $"https://api.aladhan.com/v1/timingsByCity?city={city}&country={country}&method={method}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle errors here
                return null;
            }
        }
    }

}
