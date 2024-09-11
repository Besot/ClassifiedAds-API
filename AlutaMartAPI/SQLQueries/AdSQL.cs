namespace AlutaMartAPI.SQLQueries;
    public class AdSQL
    {
         public static string UpdateAd => @"UPDATE ""Ads"" set ""Title"" = @title, ""Description"" = @description, ""Price"" = @price,
                ""DiscountPrice"" = @discountPrice, ""IsFeatured"" = @isFeatured, ""AdsType"" = @adsType, ""QuantityInStock"" = @quantityInStock,
                ""AdsCondition"" = @adsCondition, ""AdsCategoryId"" = @adsCategoryId, ""CurrencyId"" = @currencyId, ""Discount"" = @discount
            WHERE ""Id"" = @id";

        public static string DeleteAd => @"UPDATE ""Ads"" set ""Deleted"" = now(), ""IsDeleted"" = true  WHERE ""Id"" = @adId";

    }