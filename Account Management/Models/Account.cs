namespace Account_Management.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public int? ParentAccountId { get; set; }
        public bool IsActive { get; set; }
    }
}
