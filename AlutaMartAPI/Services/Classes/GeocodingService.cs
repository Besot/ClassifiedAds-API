using AlutaMartAPI.Utilities;
using Newtonsoft.Json.Linq;

namespace AlutaMartAPI.Services;
public class GeocodingService(HttpClient httpClient, string apiKey) : IGeocodingService
{
    private readonly HttpClient _httpClient = httpClient;
        private readonly string _apiKey = apiKey;

    public async Task<(double Latitude, double Longitude)> GetCoordinates(string address)
    {
        // Format the address for URL encoding
        string formattedAddress = Uri.EscapeDataString(address);

        // Geocoding API endpoint (adjust based on the correct API you're using)
        string requestUrl = $"https://geocode.maps.co/search?q={formattedAddress}&api_key={Constants.ApiKey}";

        // Send HTTP request to Geocoding API
        HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error calling Geocoding API: {response.StatusCode}");
        }
        string responseContent = await response.Content.ReadAsStringAsync();

        // Parse the JSON response - handle array response
        var jsonArray = JArray.Parse(responseContent);
        if (!jsonArray.Any())
        {
            throw new Exception("No location found for the specified address.");
        }
        // Assuming the first result is the most relevant
        var firstResult = jsonArray[0];
        var latitude = (double?)firstResult["lat"];
        var longitude = (double?)firstResult["lon"];

        if (latitude == null || longitude == null)
        {
            throw new Exception("Latitude or Longitude information is missing.");
        }
        return (latitude.Value, longitude.Value);
    }
}