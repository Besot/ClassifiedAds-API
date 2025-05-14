namespace AlutaMartAPI.SQLQueries;
    public class CartSQL
    {

        public static string RemoveAdFromCart => @"UPDATE ""Carts"" set ""Deleted"" = now(), ""IsDeleted"" = true  WHERE ""AdId"" = @adId AND ""ProfileId"" = @ProfileId";

    }