using Account_Management.Models;
using Account_Management.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Account_Management.Pages.ChartOfAccounts
{
    public class IndexModel : PageModel
    {
        private readonly DatabaseService _databaseService;

        public IndexModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<AccountNode> AccountTree { get; set; }

        public class AccountNode
        {
            public Account Account { get; set; }
            public List<AccountNode> Children { get; set; } = new List<AccountNode>();
        }

        public void OnGet()
        {
            var accounts = _databaseService.GetChartOfAccounts();
            AccountTree = BuildAccountTree(accounts, null);
        }

        private List<AccountNode> BuildAccountTree(List<Account> accounts, int? parentId)
        {
            return accounts
                .Where(a => a.ParentAccountId == parentId)
                .Select(a => new AccountNode
                {
                    Account = a,
                    Children = BuildAccountTree(accounts, a.AccountId)
                })
                .ToList();
        }

       
    }
}
