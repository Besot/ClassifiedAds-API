namespace AlutaMartAPI.SQLQueries;
    public class AdSQL
    {
         public static string UpdateAd => @"UPDATE ""Ads"" set ""Title"" = @title, ""Description"" = @description, ""Price"" = @price,
                ""DiscountPrice"" = @discountPrice, ""IsFeatured"" = @isFeatured, ""AdsType"" = @adsType, ""QuantityInStock"" = @quantityInStock,
                ""AdsCondition"" = @adsCondition, ""AdsCategoryId"" = @adsCategoryId, ""CurrencyId"" = @currencyId, ""Discount"" = @discount
            WHERE ""Id"" = @id";

        public static string DeleteAd => @"UPDATE ""Ads"" set ""Deleted"" = now(), ""IsDeleted"" = true  WHERE ""Id"" = @adId";

        public static string SetIsFeaturedFalseForExpiredAds = @"
                WITH ExpiredAds AS (
                    SELECT ""Id""
                    FROM ""Ads""
                    WHERE ""FeaturedExpiryDate"" < now() AND ""IsFeatured"" = true
                    LIMIT @batchSize
                )
                UPDATE ""Ads""
                SET ""IsFeatured"" = false
                WHERE ""Id"" IN (SELECT ""Id"" FROM ExpiredAds);
                ";

         public static string UpdateAdEngagement => @"UPDATE ""AdsEngagements"" SET ""VisitCount"" = @VisitCount, 
                ""IsPurchased"" = CASE WHEN @IsPurchased = true THEN true ELSE ""IsPurchased"" END 
                WHERE ""AdId"" = @AdId AND ""ProfileId"" = @ProfileId";
    }