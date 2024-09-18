namespace AlutaMartAPI.ModelObjects;

public class DashboardAdsOverViewDTO
{
    public string Month { get; set; }
    public int TotalAds { get; set; }
    public int PopularAds { get; set; }

    public int BottomRankedAds { get; set; }
    public double AdsConversionRate { get; set; }
}