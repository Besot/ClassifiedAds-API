using Newtonsoft.Json.Linq;

namespace AlutaMartAPI.Services;
public class GeocodingService(HttpClient httpClient, string googleApiKey) : IGeocodingService
{
    private readonly HttpClient _httpClient = httpClient;
        private readonly string _googleApiKey = googleApiKey;

    public async Task<(double Latitude, double Longitude)> GetCoordinates(string address)
    {
         // Format the address for URL encoding
            string formattedAddress = Uri.EscapeDataString(address);

            // Google Geocoding API endpoint
            string requestUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={formattedAddress}&key={_googleApiKey}";

            // Send HTTP request to Google API
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error calling Google Geocoding API: {response.StatusCode}");
            }

            string responseContent = await response.Content.ReadAsStringAsync();

            // Parse the JSON response
            JObject jsonResponse = JObject.Parse(responseContent);
            var location = jsonResponse["results"]?[0]?["geometry"]?["location"];

            if (location == null)
            {
                throw new Exception("No location found for the specified address.");
            }

            double latitude = (double)location["lat"];
            double longitude = (double)location["lng"];

            return (latitude, longitude);
    }
}