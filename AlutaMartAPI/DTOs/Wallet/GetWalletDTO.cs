namespace AlutaMartAPI.ModelObjects;
    public class GetWalletDTO
    {
        public double Balance { get; set; }
        public int TotalSales { get; set; }
        public List<GetWalletTransactionDTO> TransactionHistory { get; set; }
        
    }