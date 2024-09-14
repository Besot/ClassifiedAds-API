using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.ModelObjects;
    public class GetWalletTransactionDTO
    {
        public string Item { get; set; }
        public string InitiatorName { get; set; }
        public double Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTimeOffset? DatePaid { get; set; }
    }