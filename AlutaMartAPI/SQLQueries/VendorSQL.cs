namespace AlutaMartAPI.SQLQueries;
    public class VendorSQL
    {
	    public static string DeleteVendor => @"UPDATE ""Vendors"" set  ""Modified"" = now(), ""Deleted"" = now(), ""IsDeleted"" = true  WHERE ""ProfileId"" = @profileId";
    }
