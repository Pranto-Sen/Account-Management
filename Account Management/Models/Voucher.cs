namespace Account_Management.Models
{
    public class Voucher
    {
        public int VoucherId { get; set; }
        public string VoucherType { get; set; } // Journal, Payment, Receipt
        public DateTime VoucherDate { get; set; }
        public string ReferenceNo { get; set; }
        public List<VoucherEntry> Entries { get; set; } = new List<VoucherEntry>();
    }
}
