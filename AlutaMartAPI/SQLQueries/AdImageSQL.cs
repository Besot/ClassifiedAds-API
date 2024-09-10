namespace AlutaMartAPI.SQLQueries;
    public class AdImageSQL
    {
        public static string DeleteAdImages => @"UPDATE ""AdsImages"" set ""Deleted"" = now(), ""IsDeleted"" = true
            WHERE ""AdsId"" = @adsId AND ""ImageUrl"" IN ({adimageIds})";
     
    }