namespace AlutaMartAPI.SQLQueries;
    public static class ProfileSQL
    {
	public static string VerifyUserAccount => @"UPDATE ""Profiles"" set ""IsActive"" = true, ""Modified"" = now(), ""EmailConfirmed"" = true,
	 	""Token"" = 0, ""PhoneNumberConfirmed"" = true WHERE ""Token"" = @token";

	public static string DeleteToken => @"UPDATE ""Profiles"" set ""Modified"" = now(), ""Token"" = 0 WHERE ""Id"" = @id";

	public static string SetAsVendor => @"UPDATE ""Profiles"" set ""Modified"" = now(), ""Role"" = 2 WHERE ""Id"" = @id";

	public static string UpdateToken => @"UPDATE ""Profiles"" set ""Token"" = @token, ""TokenResetTime"" = now(), ""Modified"" = now() WHERE ""Id"" = @id";
}