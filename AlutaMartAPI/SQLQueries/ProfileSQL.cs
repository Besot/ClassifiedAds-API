namespace AlutaMartAPI.SQLQueries;
    public static class ProfileSQL
    {
	public static string VerifyUserAccount => @"UPDATE ""Profiles"" set ""IsActive"" = true, ""Modified"" = now(), ""EmailConfirmed"" = true,
	 	""Token"" = 0, ""PhoneNumberConfirmed"" = true WHERE ""Token"" = @token";

	public static string DeleteToken => @"UPDATE ""Profiles"" set ""Modified"" = now(), ""Token"" = 0 WHERE ""Id"" = @id";

	public static string SetAsVendor => @"UPDATE ""Profiles"" set ""Modified"" = now(), ""Role"" = 2 WHERE ""Id"" = @id";

	public static string UpdateToken => @"UPDATE ""Profiles"" set ""Token"" = @token, ""TokenResetTime"" = now(), ""Modified"" = now() WHERE ""Id"" = @id";

	public static string SetProfileInactive => @"UPDATE ""Profiles"" SET ""IsActive"" = false WHERE ""Id"" = @id";

	public static string SetProfileActive => @"UPDATE ""Profiles"" SET ""IsActive"" = true WHERE ""Id"" = @id";
	public static string UpdateAdPurchasedCount => @"UPDATE ""Profiles"" SET ""AdPurchasedCount"" = ""AdPurchasedCount"" + @quantity, ""Modified"" = now() WHERE ""Id"" = @profileId";
        
}