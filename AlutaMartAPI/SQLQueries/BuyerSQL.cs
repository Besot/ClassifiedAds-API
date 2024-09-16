
namespace AlutaMartAPI.SQLQueries;
    public class BuyerSQL
    {
        public static string UpdateAdPurchasedCount => 
                @"UPDATE ""Buyers"" SET ""AdPurchasedCount"" = ""AdPurchasedCount"" + @quantity, ""Modified"" = now() WHERE ""Id"" = @buyerId";
        
        public static string InsertBuyer =>
            @"INSERT INTO ""Buyers"" (""Id"", ""ProfileId"", ""AdPurchasedCount"", ""Created"", ""Modified"", ""IsDeleted"", ""IsActive"")
              VALUES (gen_random_uuid(), @profileId, 0, now(), now(), false, true);";

    }