namespace AlutaMartAPI.SQLQueries;

public class TransactionSQL
{
    public static string SetPaymentInflowAsFailed => @"UPDATE ""PaymentInflows"" set ""Status"" = 5, ""Modified"" = now() WHERE ""Id"" = @id";

    public static string SetPaymentInflowAsSuccessful => @"UPDATE ""PaymentInflows"" set ""Status"" = 3, ""DatePaid"" = now(), ""Modified"" = now() WHERE ""Id"" = @id";

    public static string UpdateWalletBalance => @"UPDATE ""Wallets"" set ""Amount"" = ""Amount"" + @amountPaid, ""Modified"" = now() WHERE ""Id"" = @id";
}
