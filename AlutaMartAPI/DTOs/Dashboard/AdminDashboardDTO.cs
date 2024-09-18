namespace AlutaMartAPI.ModelObjects;

public class AdminDashboardDTO
{
    public int CompletedLiveClasses { get; set; }
    public int PreRecordedCourses { get; set; }

    public int ActiveFeatureAdsMonthly { get; set; }
     public int ActiveFeatureAdsQuarterly { get; set; }

    public int TotalAds { get; set; }
    public int PopularAds { get; set; }
    public int FeaturedAds { get; set; }
    public int DiscountedAds { get; set; }

    public int BottomRankedAds { get; set; }
    public double AdsConversionRate { get; set; }

    public int TotalVendors { get; set; }
    public int ActiveVendors { get; set; }
    public int InActiveVendors { get; set; }

    public int TotalBuyers { get; set; }
    public int ActiveBuyers { get; set; }
    public int InActiveBuyers { get; set; }

    public int TotalAdmins { get; set; }
    public int ActiveAdmins { get; set; }
    public int InActiveAdmins { get; set; }

    public List<DashboardAdsOverViewDTO> AdsOverView { get; set; }
}