
namespace AlutaMartAPI.SQLQueries;
    public class BuyerSQL
    {
        public static string UpdateAdPurchasedCount => 
                @"UPDATE ""Buyers"" SET ""AdPurchasedCount"" = ""AdPurchasedCount"" + @quantity WHERE ""Id"" = @buyerId";

    }