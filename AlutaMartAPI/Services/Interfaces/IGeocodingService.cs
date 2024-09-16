namespace AlutaMartAPI.Services;

    public interface IGeocodingService
    {
        Task<(double Latitude, double Longitude)> GetCoordinates(string address);
    }