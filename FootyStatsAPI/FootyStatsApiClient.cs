using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FootyStatsAPI
{
    public class FootyStatsApiClient
    {
        private readonly string _baseUrl = "https://api.footystats.org/premium/v2/";
        private readonly string _apiKey;

        public FootyStatsApiClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<List<PremiumPrediction>> GetPremiumPredictionsAsync()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();

                var response = await httpClient.GetAsync($"{_baseUrl}predictions?key={_apiKey}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to get Premium Predictions from FootyStats API: {response.StatusCode}");
                }

                var result = JsonConvert.DeserializeObject<List<PremiumPrediction>>(await response.Content.ReadAsStringAsync());

                return result;
            }
        }
    }
}
